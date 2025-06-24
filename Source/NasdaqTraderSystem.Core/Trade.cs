using NasdaqTrader.Bot.Core;

namespace NasdaqTraderSystem.Core;

public class Trade
{
    public DateOnly ExecutedOn { get; set; }
    public StockListing Listing { get; set; }
    public int Amount { get; set; }
    public decimal AtPrice { get; set; }
    public decimal Total => Amount * AtPrice;
}