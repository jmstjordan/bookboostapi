using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookBoostApi.Models;


public class AdUpload
{
    public required DateOnly AdDate { get; set; }

    public required Genre Genre { get; set; }

    public required string ProductId { get; set; }

}

public class Ad : AdUpload
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public required string User { get; set; }
}
