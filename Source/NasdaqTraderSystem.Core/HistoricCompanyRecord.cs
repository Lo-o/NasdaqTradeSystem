using NasdaqTrader.Bot.Core;

namespace NasdaqTraderSystem.Core;

public class HistoricCompanyRecord
{
    public string Name { get; set; }
    public Holding[] Holdings { get; set; }
    public decimal Cash { get; set; }
    public DateOnly OnDate { get; set; }
    public Trade[] Transactions { get; set; }

    public decimal TotalWorth
    {
        get => Cash + TotalHolding;
    }

    public decimal TotalHolding => Holdings.Sum(c =>
        c.Amount * c.Listing.PricePoints.FirstOrDefault(c => c.Date == OnDate)?.Price ??
        c.Listing.PricePoints.MinBy(
            c => c.Date.ToDateTime(new TimeOnly(12, 0)) - OnDate.ToDateTime(new TimeOnly(12, 0))).Price);
}