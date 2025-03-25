namespace BookBoostApi.Services;

using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;

public class ProductService : IProductService
{
    private IAmazonProductService _amazonProductService;
    private IMemoryCache _memoryCache;
    private IAiService _aiService;
    private ILogger<ProductService> _logger;

    public ProductService(ILogger<ProductService> logger, IAmazonProductService amazonProductService, IMemoryCache memoryCache, IAiService aiService)
    {
        _amazonProductService = amazonProductService;
        _memoryCache = memoryCache;
        _aiService = aiService;
        _logger = logger;

        // // and any other products, apple, google, barns and noble, etc.
    }

    public async Task<Product> GetProduct(ProductUpload product)
    {
        string key = product.GetCacheKey();
        if (!_memoryCache.TryGetValue(product.GetCacheKey(), out Product cacheValue))
        {
            _logger.LogInformation($"Cache Miss: {key}");
            switch(product.ProductSource)
            {
                case ProductSource.Amazon:
                    var amazonProduct = await _amazonProductService.GetProduct(product.ProductId);
                    if(amazonProduct.Description != null)
                    {
                        amazonProduct.DescriptionView = await _aiService.TrimDescription(amazonProduct.Description, 250);
                    }
                    amazonProduct.User = "jmjordan";
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

    public async Task<IEnumerable<Product>> GetProducts(ProductSearch productSearch)
    {
        string key = productSearch.GetCacheKey();
        if (!_memoryCache.TryGetValue(productSearch, out List<Product> cacheValue))
        {
            _logger.LogInformation($"Cache Miss: {key}");
            var results = await _amazonProductService.GetProducts(productSearch);

            // TODO: add search params
            _memoryCache.Set(productSearch, results,  new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(1)));
            return results;
        }
        _logger.LogInformation($"Cache Hit {key}");
        return cacheValue;
    }
}