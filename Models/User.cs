using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookBoostApi.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string? Username { get; set; }

    [BsonIgnoreIfNull]
    public string? Name { get; set; }

    [BsonIgnoreIfNull]
    public string? ProfilePicture { get; set; }

    public required string Email { get; set; }
    
    [JsonIgnore]
    public string? PasswordHash { get; set;}

    [BsonRepresentation(BsonType.String)]
    public Role Role { get; set; }

    [JsonIgnore]
    public bool IsAdmin { get; set; }

    [BsonIgnoreIfNull]
    public string? CustomerId { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    [BsonRepresentation(BsonType.ObjectId)]
    public required string SubscriberId { get; set; }

}

public enum Role
{
    Reader,
    Author
}