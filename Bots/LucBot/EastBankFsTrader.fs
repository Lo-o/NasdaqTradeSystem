module EastBankFsTrader

open System.Threading.Tasks
open NasdaqTrader.Bot.Core

type MyTraderBot() =

    // Define a private field or property for the company name
    let companyName = "FSharp Traders Inc."

    interface ITraderBot with

        member _.DoTurn(systemContext: ITraderSystemContext) : Task =
            // Implement your async logic here
            task {
                // Do something with systemContext
                return ()
            } :> Task

        member _.CompanyName = companyName
