using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookBoostApi.Models;

public enum ProductSource
{
    Amazon
}

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [JsonIgnore]
    public DateTime Created { get; set; } = DateTime.Now;

    public required string ProductId { get; set; }   

    public required ProductSource ProductSource { get; set; }

    public required string Title { get; set; }

    [JsonIgnore]
    public string? Description { get; set; }

    public string? DescriptionView { get; set; }

    public string? Link { get; set; }

    public Price? Price { get; set; }

    public double Rating { get; set; }

    public int NumReviews { get; set; }

    public string? Image { get; set; }

    public string GetCacheKey()
    {
        return this.ProductId + this.ProductSource;
    }
}

public class ProductUpload
{
    public required string ProductId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public required ProductSource ProductSource { get; set; }

    public string GetCacheKey()
    {
        return this.ProductId + this.ProductSource;
    }
}

public class ProductSearch
{
    // amazon kindle
    public required string CategoryId { get; set; } = "133140011";

    // TODO: change this to ENUM
    public string? SortBy { get; set; }

    public string? SearchTerm { get; set; }

    public string GetCacheKey()
    {
        return $"{this.CategoryId}_{this.SortBy}_{this.SearchTerm}";
    }
}