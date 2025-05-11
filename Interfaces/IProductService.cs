
using BookBoostApi.Models;

namespace BookBoostApi.Interfaces;

public interface IProductService
{
    public Task<IEnumerable<Product>> GetProducts();

    public Task LoadProducts(ProductSearch productSearch);

    public Task<Product> GetProductFromSource(ProductValidate product);

    public Task<Product> GetProduct(string id);

    public Task<IEnumerable<Product>> GetProductsByUser(string userId);

    public IEnumerable<ProductSource> GetProductSources();

    public Task<Product> CreateProduct(ProductUpload product, string userId);

    public Task AddProduct(Product product);
}

