namespace BookBoostApi.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class TokenStore
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsRevoked { get; set; } = false;
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public TokenType TokenType { get; set;}
}

public class TokenRequest
{
    public string Token { get; set; }
}

public enum TokenType
{
    Refresh,
    PasswordReset
}

public class ForgotPasswordRequest
{
    public string Email { get; set; }
}

public class ResetPasswordRequest
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}