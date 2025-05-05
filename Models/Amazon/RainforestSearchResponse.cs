namespace BookBoostApi.Models;

using System.Text.Json.Serialization;

public class RainforestSearchResponse
{
    [JsonPropertyName("search_results")]
    public IEnumerable<RainforestSearchResult> SearchResults { get; set; }

}

public class RainforestSearchResult
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("link")]
    public string Link { get; set; }

    [JsonPropertyName("rating")]
    public double Rating { get; set; }

    [JsonPropertyName("ratings_total")]
    public int RatingsTotal { get; set; }

    [JsonPropertyName("asin")]
    public string Asin { get; set; }

    [JsonPropertyName("image")]
    public string Image { get; set; }

    [JsonPropertyName("price")]
    public Price Price { get; set; }

}

public class Price
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; }

    [JsonPropertyName("list_price")]
    public string ListPrice { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [JsonPropertyName("raw")]
    public string Raw { get; set; }

    [JsonPropertyName("value")]
    public double Value { get; set; }

    public int? GetPrice()
    {
        return Convert.ToInt32(Value * 100);
        if(Value > 0)
        {
            // cents
            return Convert.ToInt32(Value * 100);
        }
        var price = ParseStringPrice(ListPrice);
        if(price != null)
        {
            return price;
        }
        price = ParseStringPrice(Raw);
        if(price != null)
        {
            return price;
        }
        return null;
    }

    public int? ParseStringPrice(string amount)
    {
        if (string.IsNullOrWhiteSpace(amount))
        {
            return null;
        }

        // Remove dollar signs and trim
        var cleaned = amount.Replace("$", "").Trim();

        // Try parsing as decimal
        if (decimal.TryParse(cleaned, out decimal value))
        {
            return Convert.ToInt32(value * 100);
        }
        return null;
    }

}