namespace BookBoostApi.Services;

using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class ProductService : IProductService
{
    private readonly IMongoCollection<Product> _productsCollection;
    private IAmazonProductService _amazonProductService;
    private IMemoryCache _memoryCache;
    private IAiService _aiService;
    private ILogger<ProductService> _logger;

    public ProductService(ILogger<ProductService> logger, IAmazonProductService amazonProductService, IMemoryCache memoryCache, IAiService aiService, IOptions<BookBoostDatabaseSettings> settings)
    {
        _amazonProductService = amazonProductService;
        _memoryCache = memoryCache;
        _aiService = aiService;
        _logger = logger;

        var mongoClient = new MongoClient(
            settings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            settings.Value.DatabaseName);

        _productsCollection = mongoDatabase.GetCollection<Product>(
            settings.Value.ProductsCollectionName);

        // // and any other products, apple, google, barns and noble, etc.
    }

    public async Task<Product> GetProductFromSource(ProductUpload product)
    {
        string key = product.GetCacheKey();
        if (!_memoryCache.TryGetValue(key, out Product cacheValue))
        {
            _logger.LogInformation($"Cache Miss: {key}");
            switch(product.ProductSource)
            {
                case ProductSource.Amazon:
                    var amazonProduct = await _amazonProductService.GetProduct(product.ProductId);
                    if(amazonProduct == null)
                    {
                        return null;
                    }
                    if(amazonProduct.Description != null)
                    {
                        amazonProduct.DescriptionTrim = await _aiService.TrimDescription(amazonProduct.Description, 250);
                    }
                    _memoryCache.Set(key, amazonProduct, new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromDays(1)));
                    return amazonProduct;
                default:
                    throw new NotImplementedException("Product Source Not Implemented");
            }
        }
        _logger.LogInformation($"Cache Hit {key}");
        return cacheValue;
    }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        string key = "get_products";
        if (!_memoryCache.TryGetValue(key, out List<Product> cacheValue))
        {
            _logger.LogInformation($"Cache Miss: {key}");
            var results = await _productsCollection.Find(FilterDefinition<Product>.Empty).ToListAsync();

            // TODO: add search params
            _memoryCache.Set(key, results,  new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(1)));
            return results;
        }
        _logger.LogInformation($"Cache Hit {key}");
        return cacheValue;
    }

    public IEnumerable<ProductSource> GetProductSources()
    {
        return (ProductSource[])Enum.GetValues(typeof(ProductSource));
    }

    public async Task LoadProducts(ProductSearch productSearch)
    {
        var results = await _amazonProductService.GetProducts(productSearch);
        await _productsCollection.InsertManyAsync(results);
    }

    public async Task<Product> CreateProduct(ProductUpload product, string? userId = null)
    {
        var newProduct = await GetProductFromSource(product);
        if(newProduct == null)
        {
            return null;
        }
        newProduct.OfferPrice = product.OfferPrice;
        if(product.DescriptionView != null)
        {
            newProduct.DescriptionView = product.DescriptionView;
        }
        if(product.TitleView != null)
        {
            newProduct.TitleView = product.TitleView;
        }
        if(userId != null)
        {
            newProduct.UserId = userId;
        }
        await _productsCollection.InsertOneAsync(newProduct);
        return newProduct;
    }

    public async Task<Product> GetProduct(string id)
    {
        return await _productsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByUser(string userId)
    {
        return await _productsCollection.Find(x => x.UserId == userId).ToListAsync();
    }
}