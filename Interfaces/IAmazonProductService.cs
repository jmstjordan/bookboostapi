
using BookBoostApi.Models;

namespace BookBoostApi.Interfaces;

public interface IAmazonProductService
{
    public Task<Product> GetProduct(string asin);

    public Task<IEnumerable<Product>> GetProducts(ProductSearch productSearch);
}

