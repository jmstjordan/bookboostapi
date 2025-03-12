
using MongoDB.Bson.Serialization.Attributes;

namespace BookBoostApi.Models;


public class NotificationTemplate
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public required string Id { get; set; } 
    
    public required string Content { get; set; }

    public required string Subject { get; set; }

    public required TemplateType Type { get; set; }

}

public enum TemplateType
{
    AdCreated = 0,
    AdAccepted = 1,
    AdDenied = 2,
    PromotionJoined = 3,
    UserCreated = 4
}
