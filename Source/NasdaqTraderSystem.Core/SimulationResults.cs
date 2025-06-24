using NasdaqTrader.Bot.Core;

namespace NasdaqTraderSystem.Core;

public class SimulationResults
{
    public string RunAt { get; set; }
    public StockListing[] Listings { get; set; } = Array.Empty<StockListing>();
    public CompanyResult[] Companies { get; set; } = Array.Empty<CompanyResult>();
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal StartCash { get; set; }
    public Dictionary<ITraderBot, List<Trade>> Trades { get; set; }
    public string GameName { get; set; }
}

public class CompanyResult
{
    public string Name { get; set; } = "";
    public decimal Cash { get; set; }
    public Holding[] Holdings { get; set; } = Array.Empty<Holding>();
    public DateOnly OnDate { get; set; }
    public decimal HoldingsValue { get; set; }
    public decimal Total { get; set; }
}