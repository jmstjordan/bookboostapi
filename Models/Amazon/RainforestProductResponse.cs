namespace BookBoostApi.Models;

using System.Text.Json.Serialization;

public class RainforestProductResponse
{
    [JsonPropertyName("product")]
    public RainforestProduct Product { get; set; }

}

public class RainforestProduct
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("book_description")]
    public string BookDescription { get; set; }

    [JsonPropertyName("link")]
    public string Link { get; set; }

    [JsonPropertyName("rating")]
    public double Rating { get; set; }

    [JsonPropertyName("ratings_total")]
    public int RatingsTotal { get; set; }

    [JsonPropertyName("main_image")]
    public MainImage MainImage { get; set; }

    [JsonPropertyName("variants")]
    public List<Variant> Variants { get; set; }

}

public class Variant
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("price")]
    public Price Price { get; set; }

    [JsonPropertyName("asin")]
    public string Id { get; set; }
}

public class MainImage
{
    [JsonPropertyName("link")]
    public string Link { get; set; }
}