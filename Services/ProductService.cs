namespace BookBoostApi.Services;

using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

public class ProductService : IProductService
{
    private IAmazonProductService _amazonProductService;
    private readonly IMongoCollection<Product> _productsCollection;
    private IMemoryCache _memoryCache;

    public ProductService(IOptions<BookBoostDatabaseSettings> bookBoostDatabaseSettings, IAmazonProductService amazonProductService, IMemoryCache memoryCache)
    {
        _amazonProductService = amazonProductService;
        _memoryCache = memoryCache;

        // // and any other products, apple, google, barns and noble, etc.

        var mongoClient = new MongoClient(
            bookBoostDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookBoostDatabaseSettings.Value.DatabaseName);

        _productsCollection = mongoDatabase.GetCollection<Product>(
            bookBoostDatabaseSettings.Value.ProductsCollectionName);
    }

    public async Task<Product> CreateProduct(ProductUpload product)
    {
        switch(product.ProductSource)
        {
            case ProductSource.Amazon:
                if(ProductExistsByUser("jmjordan", product.ProductId, product.ProductSource))
                {
                    throw new ConflictException("Document already exists");
                }
                var amazonProduct = await _amazonProductService.GetProduct(product.ProductId);
                amazonProduct.UploadDate = DateOnly.FromDateTime(DateTime.Now);
                amazonProduct.User = "jmjordan";
                await _productsCollection.InsertOneAsync(amazonProduct);
                return amazonProduct;
            default:
                throw new NotImplementedException("Product Source Not Implemented");
        }
    }

    public async Task<long> DeleteProduct(string id)
    {
        var result = await _productsCollection.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount;
    }

    public async Task<Product> GetProduct(string id)
    {
        return await _productsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    private bool ProductExistsByUser(string user, string productId, ProductSource productSource)
    {
        return _productsCollection.CountDocuments(x => x.ProductId == productId 
            && x.User == user 
            && x.ProductSource == productSource
        ) > 0;
    }

    public async Task<IEnumerable<Product>> GetProducts(ProductSearch productSearch)
    {
        if (!_memoryCache.TryGetValue(productSearch, out List<Product> cacheValue))
        {
            var results = await _amazonProductService.GetProducts(productSearch);

            // TODO: add search params
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(1));

            _memoryCache.Set(productSearch, results, cacheEntryOptions);
            return results;
        }
        return cacheValue;
    }

    public async Task<IEnumerable<Product>> GetProducts(string user)
    {
        return await _productsCollection.Find(x => x.User == user).ToListAsync();
    }
}