using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookBoostApi.Models;

public class AdUpload
{
    public required DateOnly AdDate { get; set; }

    public required ProductUpload ProductUpload { get; set; } 

    public required Genre Genre { get; set; }
}

public class AdPatch
{
    public required AdState State { get; set; }
}

public enum Tier
{
    Basic,
    Ambassador
}

public class Ad
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public required string OrderId { get; set; }

    public required DateOnly AdDate { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public required string UserId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public required string ProductId { get; set; }

    public DateTime Created { get; set; } = DateTime.Now;

    public DateTime? RunDate { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public required AdState State { get; set; } 

    [BsonRepresentation(BsonType.String)]
    public required Genre Genre { get; set; }

    public required string SessionId { get; set; }

    // in cents, the amount the ad will be charged for
    public required int Price { get; set; }

    public string? PaymentMethodId { get; set; }

    public string? PaymentIntentId { get; set; }
}

public enum AdState
{
    Pending,
    Declined,
    Accepted,
    Canceled
}

public class AdAvailability
{
    public required string AdDate { get; set; }

    public required int Count { get; set; }
}