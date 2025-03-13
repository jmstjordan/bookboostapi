namespace BookBoostApi.Interfaces;

public interface IAppEmailService
{
    public Task<bool> SendAdCreated(string user);

    public Task<bool> SendAdAccepted(string user);

    public Task<bool> SendAdDeclined(string user);

    public Task<bool> SendUserCreated(string user);

    public Task<bool> SendPromotionJoined(string user);
}