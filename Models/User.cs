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

    public string? Name { get; set; }

    public string? ProfilePicture { get; set; }

    public required string Email { get; set; }
    
    [JsonIgnore]
    public string? PasswordHash { get; set;}

    public Preferences? Preferences { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Role Role { get; set; }

    [JsonIgnore]
    public bool IsAdmin { get; set; }

    public string? CustomerId { get; set; }

    public DateTime Created { get; set; } = DateTime.Now;
}

public class Preferences
{
    public Genre[]? Genres { get; set; }
}

public enum Role
{
    Reader = 1,
    Author = 2
}