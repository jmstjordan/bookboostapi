using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace BookBoostApi.Models;


public class AdUpload
{
    public required DateOnly AdDate { get; set; }

    public required Genre Genre { get; set; }

    public required string ProductId { get; set; }

}

public class Ad : AdUpload
{
    [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public required string User { get; set; }
}
