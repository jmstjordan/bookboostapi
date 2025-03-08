
using BookBoostApi.Models;

namespace BookBoostApi.Interfaces;

public interface IProductService
{
    public Task<IEnumerable<Product>> GetProducts(ProductSearch productSearch);

    public Task<IEnumerable<Product>> GetProducts(string user);

    public Task<Product> GetProduct(string id);

    public Task<Product> CreateProduct(ProductUpload product);

    public Task<long> DeleteProduct(string id);
}

