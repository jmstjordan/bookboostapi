using BookBoostApi.Interfaces;
using BookBoostApi.Models;

namespace BookBoostApi.Services;

public class PriceService : IPriceService
{
    public Dictionary<Genre, int> GetAdPrices()
    {
        var prices = new Dictionary<Genre, int>
        {
            {Genre.Romance, 5000},
            {Genre.Fantasy, 5000},
            {Genre.MysteryThriller, 5000},
            {Genre.ScienceFiction, 5000},
            {Genre.YoungAdult, 5000},
            {Genre.NonFiction, 5000}
        };
        return prices;
    }

    public IEnumerable<int> GetProductPrices()
    {
        return [99, 199, 299, 399];
    }
}