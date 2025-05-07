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
                var response = await _amazonProductService.GetProduct(product.ProductId);
                if(response == null)
                {
                    _logger.LogError($"Unable to find product from source: {product.ProductSource} {product.ProductId}");
                    throw new ProductException("Unable to find product from source");
                }
                return await BuildAmazonProduct(response, product.ProductId);
            default:
                throw new NotImplementedException("Product Source Not Implemented");
        }
    }

    private async Task<Product> BuildAmazonProduct(RainforestProductResponse response, string productId)
    {
        var variant = response.Product.Variants.Find(x => x.Id == productId);
        var newProduct = new Product
        {
            ProductId = productId,
            Title = await _aiService.TrimTitle(response.Product.Title),
            Description = await _aiService.TrimDescription(response.Product.BookDescription, 250),
            Price = variant?.Price.GetPrice(),
            NumReviews = response.Product.RatingsTotal,
            Link = response.Product.Link,
            Rating = response.Product.Rating,
            Image = response.Product.MainImage?.Link,
            ProductSource = ProductSource.Amazon,
            Author = response.Product.Authors.FirstOrDefault(),
        };
        try
        {
            if(response.Product.Categories != null && response.Product.Categories.Count > 0)
            {
                var genres = (Genre[])Enum.GetValues(typeof(Genre));
                var categories = response.Product.Categories.Select(c => c.Name).ToList();
                var selectGenres = await _aiService.GetCategories(categories, genres.Select(g => g.ToString()).ToList());
                newProduct.Genres = selectGenres.Select(s => (Genre)Enum.Parse(typeof(Genre), s)).ToList();
            }
        }
        catch(Exception e)
        {
            _logger.LogError(e.StackTrace);
            _logger.LogInformation($"Unable to get category for {productId}");
        }
        return newProduct;
    }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        string key = "get_products";
        if (!_memoryCache.TryGetValue(key, out List<Product> cacheValue))
        {
            _logger.LogInformation($"Cache Miss: {key}");
            var results = await BestProducts();
            // TODO: depriortize books that were never actually approved/paid
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
                { "Author", 1 },
                { "Genres", 1 },
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
        var products = new List<Product>();
        foreach(var response in results)
        {
            products.Add(await BuildAmazonProduct(response, response.Product.Asin));
        }
        await _productsCollection.InsertManyAsync(products);
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