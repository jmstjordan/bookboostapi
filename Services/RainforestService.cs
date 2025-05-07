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

    private ILogger<RainforestService> _logger;


    public RainforestService(IOptions<RainforestSettings> settings, ILogger<RainforestService> logger)
    {
        _apiKey = settings.Value.ApiKey;
        _logger = logger;
    }

    public async Task<RainforestProductResponse> GetProduct(string asin)
    {
        var options = new RestClientOptions(API_URL);
        var client = new RestClient(options);
        var request = new RestRequest("request")
            .AddParameter("api_key", _apiKey)
            .AddParameter("amazon_domain", AMAZON_DOMAIN)
            .AddParameter("asin", asin)
            .AddParameter("type", "product");

        // TODO: Consider cancelation token from client here as a param
        // TODO: Test when product id is not found
        RainforestProductResponse? response;
        try
        {
            response = await client.GetAsync<RainforestProductResponse>(request);
            if (response == null || response.Product == null)
            {
                return null;
            }
        }
        catch (HttpRequestException e)
        {
            _logger.LogError($"Error getting product from Rainforest API for ASIN: {asin} {e.StatusCode} {e.StackTrace}");
            return null;
        }
        return response;
    }

    public async Task<IEnumerable<RainforestProductResponse>> GetProducts(ProductSearch productSearch)
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
            .AddParameter("limit", 24)
            .AddParameter("search_term", productSearch.SearchTerm ?? DEFAULT_SEARCH_TERM);

        // TODO: Consider cancelation token from client here as a param
        RainforestSearchResponse? response;
        try
        {
            response = await client.GetAsync<RainforestSearchResponse>(request);
        }
        catch(HttpRequestException)
        {
            return new List<RainforestProductResponse>();
        }
        var products = new List<RainforestProductResponse>();
        foreach(RainforestSearchResult result in response.SearchResults)
        {
            var getProduct = await GetProduct(result.Asin);
            products.Add(getProduct);
        }
        return products;
    }
}