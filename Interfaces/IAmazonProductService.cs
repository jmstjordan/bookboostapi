
using BookBoostApi.Models;

namespace BookBoostApi.Interfaces;

public interface IAmazonProductService
{
    public Task<RainforestProductResponse> GetProduct(string asin);

    public Task<IEnumerable<RainforestProductResponse>> GetProducts(ProductSearch productSearch);
}

