using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookBoostApi.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    [JsonIgnore]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public required string Username { get; set; }

    public required string UserId { get; set; }

    public ReaderConfig? ReaderConfig { get; set; }

    public AuthorConfig? AuthorConfig { get; set; }

}

public class ReaderConfig
{
    public required Genre[] Genres { get; set; }
}

public class AuthorConfig
{

}