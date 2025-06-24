using System.Collections.ObjectModel;

namespace NasdaqTrader.Bot.Core;

public interface ITraderSystemContext
{
    DateOnly StartDate { get; set; }
    DateOnly EndDate { get; set; }
    DateOnly CurrentDate { get; set; }
    int AmountOfTradesPerDay { get; }

    decimal GetCurrentCash(ITraderBot traderBot);
    decimal GetPriceOnDay(StockListing listing);
    ReadOnlyCollection<StockListing> GetListings();
    int GetTradesLeftForToday(ITraderBot traderBot);
    bool BuyStock(ITraderBot traderBot,StockListing listing, int amount);
    bool SellStock(ITraderBot traderBot, StockListing listing, int amount);
    Holding GetHolding(ITraderBot traderBot, StockListing listing);
    Holding[] GetHoldings(ITraderBot traderBot);
}