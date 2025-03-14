namespace BookBoostApi.Services;

using System.Collections.Generic;
using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Options;
using RestSharp;

public class RainforestService : IAmazonProductService
{
    private readonly string API_URL = "https://api.rainforestapi.com";

    private readonly string AMAZON_DOMAIN = "amazon.com";

    private readonly string DEFAULT_SEARCH_TERM = "best-selling+ebooks";

    private readonly string DEFAULT_SORT_BY = "featured";

    private string _apiKey;


    public RainforestService(IOptions<RainforestSettings> settings)
    {
        _apiKey = settings.Value.ApiKey;
    }

    public async Task<Product> GetProduct(string asin)
    {
        var options = new RestClientOptions(API_URL);
        var client = new RestClient(options);
        var request = new RestRequest("request")
            .AddParameter("api_key", _apiKey)
            .AddParameter("amazon_domain", AMAZON_DOMAIN)
            .AddParameter("asin", asin)
            .AddParameter("type", "product");

        // TODO: Consider cancelation token from client here as a param
        var response = await client.GetAsync<RainforestProductResponse>(request);

        var variant = response.Product.Variants.Find(x => x.Id == asin);
        var product = new Product
        {
            ProductId = asin,
            Title = response.Product.Title,
            Description = response.Product.BookDescription,
            Price = variant?.Price,
            NumReviews = response.Product.RatingsTotal,
            Link = response.Product.Link,
            Rating = response.Product.Rating,
            Image = response.Product.MainImage?.Link,
            ProductSource = ProductSource.Amazon
        };
        return product;
    }

    public async Task<IEnumerable<Product>> GetProducts(ProductSearch productSearch)
    {
        var options = new RestClientOptions(API_URL);
        var client = new RestClient(options);

        // TODO: add more criteria on what we want the UI to show
        var request = new RestRequest("request")
            .AddParameter("api_key", _apiKey)
            .AddParameter("amazon_domain", AMAZON_DOMAIN)
            .AddParameter("category_id", productSearch.CategoryId)
            .AddParameter("type", "search")
            .AddParameter("sort_by", productSearch.SortBy ?? DEFAULT_SORT_BY)
            .AddParameter("search_term", productSearch.SearchTerm ?? DEFAULT_SEARCH_TERM);


        // TODO: Consider cancelation token from client here as a param
        RainforestSearchResponse response;
        try
        {
            response = await client.GetAsync<RainforestSearchResponse>(request);
        }
        catch(HttpRequestException e)
        {
            return new List<Product>();
        }
        var products = new List<Product>();
        foreach(RainforestSearchResult result in response.SearchResults)
        {
            products.Add(new Product
            {
                Title = result.Title,
                Link = result.Link,
                Rating = result.Rating,
                Price = result.Price,
                ProductId = result.Asin,
                Image = result.Image,
                NumReviews = result.RatingsTotal,
                ProductSource = ProductSource.Amazon
            });
        }
        return products;
    }
}