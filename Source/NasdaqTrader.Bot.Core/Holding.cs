namespace NasdaqTrader.Bot.Core;

public class Holding
{
    public StockListing Listing { get; set; }
    public int Amount { get; set; }

    public Holding Copy()
    {
        return new Holding()
        {
            Listing = Listing,
            Amount = Amount
        };
    }
}
