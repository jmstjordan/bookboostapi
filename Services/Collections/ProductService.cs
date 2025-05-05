namespace BookBoostApi.Services;

using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
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

    public async Task<Product> GetProductFromSource(ProductValidate product)
    {
        switch(product.ProductSource)
        {
            case ProductSource.Amazon:
                var amazonProduct = await _amazonProductService.GetProduct(product.ProductId);
                if(amazonProduct == null)
                {
                    _logger.LogError($"Unable to find product from source: {product.ProductSource} {product.ProductId}");
                    throw new ProductException("Unable to find product from source");
                }
                if(amazonProduct.Description != null)
                {
                    amazonProduct.Description = await _aiService.TrimDescription(amazonProduct.Description, 250);
                }
                if(amazonProduct.Title != null)
                {
                    amazonProduct.Title = await _aiService.TrimTitle(amazonProduct.Title);
                }
                return amazonProduct;
            default:
                throw new NotImplementedException("Product Source Not Implemented");
        }
    }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        string key = "get_products";
        if (!_memoryCache.TryGetValue(key, out List<Product> cacheValue))
        {
            _logger.LogInformation($"Cache Miss: {key}");
            var results = await BestProducts();
            // TODO: add search params
            _memoryCache.Set(key, results,  new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(1)));
            return results;
        }
        _logger.LogInformation($"Cache Hit {key}");
        return cacheValue;
    }

    private async Task<List<Product>> BestProducts()
    {
        var pipeline = new[]
        {
            // Step 1: Add hasUser flag (1 if UserId exists, 0 otherwise)
            new BsonDocument("$addFields", new BsonDocument("hasUser", new BsonDocument("$cond", new BsonArray {
                new BsonDocument("$ifNull", new BsonArray { "$UserId", false }),
                1,
                0
            }))),

            // Step 2: Sort by hasUser desc, Rating desc, Created desc
            new BsonDocument("$sort", new BsonDocument
            {
                { "hasUser", -1 },
                { "Rating", -1 },
                { "Created", -1 }
            }),

            // Step 3: Group by ProductId to deduplicate
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$ProductId" },
                { "doc", new BsonDocument("$first", "$$ROOT") }
            }),

            // Step 4: Replace root with the doc
            new BsonDocument("$replaceRoot", new BsonDocument("newRoot", "$doc")),

            // Step 5: Limit to 24
            new BsonDocument("$limit", 24),

            new BsonDocument("$project", new BsonDocument
            {
                { "Id", 1 },
                { "Created", 1 },
                { "ProductId", 1 },
                { "UserId", 1 },
                { "ProductSource", 1 },
                { "Title", 1 },
                { "TitleView", 1 },
                { "Description", 1 },
                { "DescriptionView", 1 },
                { "Link", 1 },
                { "Price", 1 },
                { "OfferPrice", 1 },
                { "Rating", 1 },
                { "NumReviews", 1 },
                { "Image", 1 },
                { "Author", 1 }
            })
        };

        return await _productsCollection.Aggregate<Product>(pipeline).ToListAsync();
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

    public async Task<Product> CreateProduct(ProductUpload productUpload, string? userId = null)
    {
        var product = await GetProductFromSource(productUpload);
        product.OfferPrice = productUpload.OfferPrice;
        if(productUpload.DescriptionView != null)
        {
            product.DescriptionView = productUpload.DescriptionView;
        }
        if(productUpload.TitleView != null)
        {
            product.TitleView = productUpload.TitleView;
        }
        if(userId != null)
        {
            product.UserId = userId;
        }
        await _productsCollection.InsertOneAsync(product);
        return product;
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