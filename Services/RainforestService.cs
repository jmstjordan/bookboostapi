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

    private readonly string DEFAULT_SEARCH_TERM = "best-selling ebooks";

    private readonly string DEFAULT_SORT_BY = "featured";

    private string _apiKey;


    public RainforestService(IOptions<ApiKeySettings> apiKeySettings)
    {
        _apiKey = apiKeySettings.Value.RainforestApiKey;
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

        var product = new Product
        {
            ProductId = asin,
            Title = response.Product.Title,
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
            .AddParameter("sort_by", productSearch.SortBy ?? DEFAULT_SORT_BY)
            .AddParameter("search_term", productSearch.SearchTerm ?? DEFAULT_SEARCH_TERM);

        // TODO: Consider cancelation token from client here as a param
        var response = await client.GetAsync<RainforestSearchResponse>(request);
        var products = new List<Product>();
        if(response == null)
        {
            return products;
        }
        foreach(RainforestSearchResult result in response.SearchResults)
        {
            products.Add(new Product
            {
                Title = result.Title,
                Link = result.Link,
                Rating = result.Rating,
                Price = result.Price,
                ProductId = result.Asin,
                ProductSource = ProductSource.Amazon
            });
        }
        return products;
    }

}