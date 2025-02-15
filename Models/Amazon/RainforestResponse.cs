namespace BookBoostApi.Models;

using System.Text.Json.Serialization;

public class RainforestResponse
{
    [JsonPropertyName("product")]
    public RainforestProduct Product { get; set; }

}

public class RainforestProduct
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("link")]
    public string Link { get; set; }

    [JsonPropertyName("rating")]
    public double Rating { get; set; }

    [JsonPropertyName("ratings_total")]
    public int RatingsTotal { get; set; }

}