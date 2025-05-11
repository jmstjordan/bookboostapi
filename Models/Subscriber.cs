using BookBoostApi.Models;

public class SubscriberUpload
{
    public required string Email { get; set; }

    public required SubscriberSource SubscriberSource { get; set; }
}

public class Subscriber : SubscriberUpload
{
    public bool IsSubscribed { get; set; } = true;

    public DateTime Created { get; set; } = DateTime.UtcNow;
}

public enum SubscriberSource
{
    BookTokClub
}