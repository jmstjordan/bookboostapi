
using BookBoostApi.Models;

namespace BookBoostApi.Interfaces;

public interface IProductService
{
    public Task<IEnumerable<Product>> GetProducts();

    public Task LoadProducts(ProductSearch productSearch);

    public Task<Product> GetProduct(ProductUpload product);

    public IEnumerable<ProductSource> GetProductSources();

}

