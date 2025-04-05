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

    public string? Username { get; set; }

    public required string Email { get; set; }
    
    [JsonIgnore]
    public string? PasswordHash { get; set;}

    public Role[] Roles { get; set; } = [Role.Reader];

    public Preferences? Preferences { get; set; }

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