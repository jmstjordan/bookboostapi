namespace BookBoostApi.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

public enum ProductSource
{
    Amazon = 1
}

public class Product
{
    // This is the id that we generate, vs the one used as a lookup, like an ASIN
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public required string ProductId { get; set; }   

    public required ProductSource ProductSource { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public string? Link { get; set; }

    public Price? Price { get; set; }

    public double Rating { get; set; }

    public int NumReviews { get; set; }

    public string? Image { get; set; }

    [BsonRepresentation(BsonType.DateTime)]
    public DateOnly? UploadDate { get; set; }

    public string? User { get; set; }
}

public class ProductUpload
{
    public required string ProductId { get; set; }

    public required ProductSource ProductSource { get; set; }
}

public class ProductSearch: IEquatable<ProductSearch>
{
    public required string CategoryId { get; set; }

    // TODO: change this to ENUM
    public string? SortBy { get; set; }

    public string? SearchTerm { get; set; }

    public bool Equals(ProductSearch? other)
    {
        return other != null 
            && this.CategoryId == other.CategoryId
            && this.SearchTerm == other.SearchTerm;
    }
}