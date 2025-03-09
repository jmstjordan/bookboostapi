using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookBoostApi.Models;


public class AdUpload
{
    public required DateOnly AdDate { get; set; }

    public required Genre Genre { get; set; }

    // Not to be confused with ProductId on the Product, this is the Product's Id... :(
    public required string ProductId { get; set; }

    public required Tier Tier { get; set; }
}

public enum Tier
{
    Basic = 1,
    Ambassador = 2
}

public class Ad : AdUpload
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public required string User { get; set; }

    public DateOnly Created { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    
}

public class AdAvailability
{
    public required string AdDate { get; set; }

    public required int Count { get; set; }
}
