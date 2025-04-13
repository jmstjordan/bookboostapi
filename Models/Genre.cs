using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookBoostApi.Models;

public enum Genre
{
    [BsonRepresentation(BsonType.String)]
    Romance,

    [BsonRepresentation(BsonType.String)]
    Fantasy,
    
    [BsonRepresentation(BsonType.String)]
    MysteryThriller,

    [BsonRepresentation(BsonType.String)]
    ScienceFiction,

    [BsonRepresentation(BsonType.String)]
    YoungAdult,

    [BsonRepresentation(BsonType.String)]
    NonFiction
}