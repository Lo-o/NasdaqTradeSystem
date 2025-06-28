module EastBankFsTrader

open System.Threading.Tasks
open NasdaqTrader.Bot.Core

type EastBankFsTrader() =

    let companyName = "EastBank Trading Inc."

    interface ITraderBot with
        member _.CompanyName = companyName

        member this.DoTurn(systemContext: ITraderSystemContext) : Task =
            task {
                // Get available cash
                let cash = systemContext.GetCurrentCash(this :> ITraderBot)
                let listings = systemContext.GetListings()

                // Check how many trades are left
                let tradesLeft = systemContext.GetTradesLeftForToday(this :> ITraderBot)

                // Get holdings
                let holdings = systemContext.GetHoldings(this :> ITraderBot)

                // Print current day info (for demo/logging)
                let currentDate = systemContext.CurrentDate
                printfn $"[{currentDate}] Cash: {cash} | Trades left: {tradesLeft} | Holdings: {holdings.Length}"

                // Example trading logic: Buy the first stock if we have cash and trades left
                if tradesLeft > 0 && listings.Count > 0 then
                    let stock = listings.[0]
                    let price = systemContext.GetPriceOnDay(stock)
                    let amountToBuy = int (cash / price)
                    if amountToBuy > 0 then
                        let success = systemContext.BuyStock(this :> ITraderBot, stock, amountToBuy)
                        if success then
                            printfn $"Bought {amountToBuy} of {stock.Name} at {price}"

                // You could also add a selling rule, like selling if the price is above a threshold
                return ()
            } :> Task

