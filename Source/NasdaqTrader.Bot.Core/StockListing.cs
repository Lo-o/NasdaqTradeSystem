using System.Globalization;

namespace NasdaqTrader.Bot.Core;

public class StockListing
{
    public string Name { get; set; } = "";
    public string Ticker { get; set; } = "";

    public PricePoint[] PricePoints { get; set; } = Array.Empty<PricePoint>();
}

public class PricePoint
{
    public DateOnly Date { get; set; }
    public decimal Price { get; set; }
    
    public string DateAsString => Date.ToDateTime(new TimeOnly(12,0)).ToString("o", CultureInfo.InvariantCulture);
    public string PriceAsString => Price.ToString("0");
    
}