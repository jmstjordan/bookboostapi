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
    Basic = 1,
    Ambassador = 2
}

public class Ad
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public required DateOnly AdDate { get; set; }

    public required string User { get; set; }

    public required Product Product { get; set; }

    public required DateOnly Created { get; set; }
    
    public required AdState State { get; set; } 

    public required Genre Genre { get; set; }
}

public enum AdState
{
    Pending = 0,
    Declined = 1,
    Accepted = 2
}

public class AdAvailability
{
    public required string AdDate { get; set; }

    public required int Count { get; set; }

    // public Dictionary<Genre, double>? Price { get; set; }
}