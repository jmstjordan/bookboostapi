
using System.Text.Json.Serialization;
using BookBoostApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class SubscriberUpload
{
    public required string Email { get; set; }

    [BsonRepresentation(BsonType.String)]
    public required SubscriberSource SubscriberSource { get; set; }
}

public class Subscriber : SubscriberUpload
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public bool IsSubscribed { get; set; } = true;

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public Genre[] Topics { get; set; } = (Genre[])Enum.GetValues(typeof(Genre));
}

public class SubscriberPatch
{
    public bool IsSubscribed { get; set; }

    public Genre[] Topics { get; set; }
}

public enum SubscriberSource
{
    BookTokClub
}