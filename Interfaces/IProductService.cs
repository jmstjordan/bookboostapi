
using BookBoostApi.Models;

namespace BookBoostApi.Interfaces;

public interface IProductService
{
    public Task<IEnumerable<Product>> GetProducts(ProductSearch productSearch);

    public Task<Product> GetProduct(ProductUpload product);

    public IEnumerable<ProductSource> GetProductSources();

}

