namespace LucBot.Bots

open System.Threading.Tasks
open NasdaqTrader.Bot.Core
open System

type PlannedTrade = {
    Stock: StockListing
    BuyDate: DateOnly
    SellDate: DateOnly
    BuyPrice: decimal
    SellPrice: decimal
    PotentialGain: decimal
}

type public EastBankFsTraderBot() =

    let mutable tradePlan: PlannedTrade list option = None

    let createPlan (listings: #seq<StockListing>) =
        printfn "Performing one-time analysis of all stocks..."
        let plan =
            listings
            |> Seq.toList
            |> List.choose (fun stock ->
                if stock.PricePoints.Length < 2 then None
                else
                    let minPricePoint = stock.PricePoints |> Seq.minBy (fun pp -> pp.Price)
                    
                    let futurePrices = 
                        stock.PricePoints 
                        |> Seq.filter (fun pp -> pp.Date > minPricePoint.Date)
                        
                    if not (Seq.isEmpty futurePrices) then
                        let maxPricePoint = futurePrices |> Seq.maxBy (fun pp -> pp.Price)
                        
                        // Only consider profitable trades
                        if maxPricePoint.Price > minPricePoint.Price then
                            let gain = maxPricePoint.Price / minPricePoint.Price
                            Some {
                                Stock = stock
                                BuyDate = minPricePoint.Date
                                SellDate = maxPricePoint.Date
                                BuyPrice = minPricePoint.Price
                                SellPrice = maxPricePoint.Price
                                PotentialGain = gain
                            }
                        else None // Price only goes down after the min point
                    else None // No future prices after the min point
            )
            |> List.sortByDescending (fun trade -> trade.PotentialGain)
            
        printfn "Analysis complete. Found %d profitable trades." (List.length plan)
        Some plan



    interface ITraderBot with
        member _.CompanyName = "Simple Greedy F# Bot"

        member this.DoTurn(context: ITraderSystemContext) : Task =
            async {
                if tradePlan.IsNone then
                    let listings = context.GetListings()
                    tradePlan <- createPlan listings

                match tradePlan with
                | None -> 
                    printfn "No trade plan available. Doing nothing."
                | Some plan ->
                    let currentDate = context.CurrentDate

                    let sellsForToday =
                        plan
                        |> List.filter (fun trade -> trade.SellDate = currentDate)

                    for tradeToSell in sellsForToday do
                        let holding = context.GetHolding(this, tradeToSell.Stock)
                        if holding <> null && holding.Amount > 0 && context.GetTradesLeftForToday(this) > 0 then
                            printfn "SELL: %d of %s on %A" holding.Amount holding.Listing.Ticker currentDate
                            context.SellStock(this, holding.Listing, holding.Amount) |> ignore

                    let buysForToday =
                        plan
                        |> List.filter (fun trade -> trade.BuyDate = currentDate)

                    for tradeToBuy in buysForToday do
                        if context.GetTradesLeftForToday(this) > 0 then
                            let cash = context.GetCurrentCash(this)
                            let price = context.GetPriceOnDay(tradeToBuy.Stock)

                            if price > 0m && cash > price then
                                let currentHolding = 
                                    let h = context.GetHolding(this, tradeToBuy.Stock)
                                    if h = null then 0 else h.Amount

                                let affordableAmount = int (cash / price)
                                let maxAmountToBuy = min affordableAmount (1000 - currentHolding)

                                if maxAmountToBuy > 0 then
                                    printfn "BUY: %d of %s on %A for %.2f each" maxAmountToBuy tradeToBuy.Stock.Ticker currentDate price
                                    context.BuyStock(this, tradeToBuy.Stock, maxAmountToBuy) |> ignore

            } 
            |> Async.StartAsTask :> Task