
using BookBoostApi.Models;

namespace BookBoostApi.Interfaces;

public interface IPriceService
{
    public Dictionary<Genre, int> GetAdPrices();

    // cents
    public IEnumerable<int> GetProductPrices();
}

