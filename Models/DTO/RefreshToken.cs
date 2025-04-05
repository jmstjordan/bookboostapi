using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class RefreshToken
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsRevoked { get; set; } = false;
    
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string UserId { get; set; }
}