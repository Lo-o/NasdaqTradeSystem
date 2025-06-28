namespace LucBot.Bots

open System.Threading.Tasks
open NasdaqTrader.Bot.Core

type public EastBankFsTraderBot() =

    interface ITraderBot with
        member _.CompanyName = "EastBank Trading Inc."

        member this.DoTurn(systemContext: ITraderSystemContext) =
            task {
                let today = systemContext.CurrentDate
                let listings = systemContext.GetListings()
                let tradesLeft = systemContext.GetTradesLeftForToday(this :> ITraderBot)
                let cash = systemContext.GetCurrentCash(this :> ITraderBot)

                // Evaluate listings by max future gain
                let scored =
                    listings
                    |> Seq.choose (fun listing ->
                        let currentPrice = systemContext.GetPriceOnDay(listing)
                        let futurePoints =
                            listing.PricePoints
                            |> Seq.filter (fun p -> p.Date >= today)
                            |> Seq.toList

                        match futurePoints with
                        | [] -> None  // Skip this stock; no future data
                        | _ ->
                            let maxFuturePrice = futurePoints |> Seq.map (fun p -> p.Price) |> Seq.max
                            let delta = maxFuturePrice - currentPrice
                            Some (listing, currentPrice, delta)
                    )
                    |> Seq.sortByDescending (fun (_, _, delta) -> delta)
                    |> Seq.toList

                let mutable trades = 0
                let mutable cashLeft = cash

                // First try to sell things that are no longer going up
                for holding in systemContext.GetHoldings(this :> ITraderBot) do
                    if trades < tradesLeft && holding.Amount > 0 then
                        let listing = holding.Listing
                        let priceNow = systemContext.GetPriceOnDay(listing)
                        let futurePrices =
                            listing.PricePoints
                            |> Seq.filter (fun p -> p.Date > today)
                            |> Seq.map (fun p -> p.Price)
                            |> Seq.toList
                        let willGoHigher = futurePrices |> List.exists (fun p -> p > priceNow)
                        if not willGoHigher then
                            let amountToSell = min holding.Amount (1000)
                            if systemContext.SellStock(this :> ITraderBot, listing, amountToSell) then
                                trades <- trades + 1
                                cashLeft <- cashLeft + decimal amountToSell * priceNow

                // Then try to buy the best opportunities
                for (listing, price, delta) in scored do
                    if trades >= tradesLeft then () else
                    let alreadyHave = systemContext.GetHolding(this :> ITraderBot, listing).Amount
                    if alreadyHave < 1000 && delta > 0m then
                        let maxCanBuy = min (1000 - alreadyHave) (int (cashLeft / price))
                        if maxCanBuy > 0 then
                            if systemContext.BuyStock(this :> ITraderBot, listing, maxCanBuy) then
                                trades <- trades + 1
                                cashLeft <- cashLeft - decimal maxCanBuy * price
            }