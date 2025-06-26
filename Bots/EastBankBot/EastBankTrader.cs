using NasdaqTrader.Bot.Core;

namespace EastBankBot;

public class EastBankTrader : ITraderBot
{
    public string CompanyName { get; } = "Eastbank trading inc.";

    public Task DoTurn(ITraderSystemContext systemContext)
    {
        var currentCash = systemContext.GetCurrentCash(this);


        return Task.CompletedTask;
    }
}
