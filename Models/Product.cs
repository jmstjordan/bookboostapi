using System.ComponentModel.DataAnnotations;
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
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [JsonIgnore]
    public DateTime Created { get; set; } = DateTime.Now;

    public required string ProductId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? UserId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public required ProductSource ProductSource { get; set; }

    public required string Title { get; set; }
    
    public string? TitleView { get; set; }

    public string? Description { get; set; }

    public string? DescriptionView { get; set; }

    public string? Link { get; set; }

    public int? Price { get; set; }

    public int? OfferPrice { get; set; }

    public double Rating { get; set; }

    public int NumReviews { get; set; }

    public string? Image { get; set; }

    public Author? Author { get; set; }

    [BsonRepresentation(BsonType.String)]
    public List<Genre> Genres { get; set; }

}

public class ProductValidate
{
    public required string ProductId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public required ProductSource ProductSource { get; set; }
}

public class ProductUpload : ProductValidate
{
    public required int OfferPrice { get; set; }

    [MaxLength(200)]
    public string? TitleView { get; set; }

    [MaxLength(1000)]
    public string? DescriptionView { get; set; }
}

public class ProductSearch
{
    // amazon kindle
    public required string CategoryId { get; set; } = "133140011";

    // TODO: change this to ENUM
    public string? SortBy { get; set; }

    public string? SearchTerm { get; set; }
}