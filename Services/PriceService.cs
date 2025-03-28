using BookBoostApi.Interfaces;
using BookBoostApi.Models;

namespace BookBoostApi.Services;

public class PriceService : IPriceService
{
    public Dictionary<Genre, int> GetPrices()
    {
        var prices = new Dictionary<Genre, int>
        {
            {Genre.Romance, 6000},
            {Genre.Fantasy, 4000},
            {Genre.MysteryThriller, 3000},
            {Genre.ScienceFiction, 2000},
            {Genre.YoungAdult, 5500},
            {Genre.NonFiction, 5100}
        };
        return prices;
    }
}