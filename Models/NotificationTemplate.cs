
using MongoDB.Bson.Serialization.Attributes;

namespace BookBoostApi.Models;


public class NotificationTemplate
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public required string Id { get; set; } 
    
    public required string Content { get; set; }

    public required string Subject { get; set; }

    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public required TemplateType Type { get; set; }

}

public enum TemplateType
{
    AdCreated,
    AdAccepted,
    AdDeclined,
    PromotionJoined,
    UserCreated,
    PasswordReset
}
