
using BookBoostApi.Interfaces;
using BookBoostApi.Models;

using RestSharp;

public class RainforestService : IAmazonProductService
{
    private readonly string API_URL = "https://api.rainforestapi.com";

    private readonly string AMAZON_DOMAIN = "amazon.com";

    private string _apiKey;

    public RainforestService(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task<AmazonProduct> GetProduct(string id)
    {
        // make rain forest specific api call
        var options = new RestClientOptions(API_URL);
        var client = new RestClient(options);
        var request = new RestRequest("request")
            .AddParameter("api_key", _apiKey)
            .AddParameter("amazon_domain", AMAZON_DOMAIN)
            .AddParameter("asin", id)
            .AddParameter("type", "product");

        // TODO: Consider cancelation token from client here as a param
        var product = await client.GetAsync<RainforestResponse>(request);
        // do some cleanup with object
        return null;
    }
}