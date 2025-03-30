using BookBoostApi.Models;

namespace BookBoostApi.Interfaces;

public interface IAppEmailService
{
    public Task<bool> SendAdCreated(string userId);

    public Task<bool> SendAdAccepted(string userId);

    public Task<bool> SendAdDeclined(string userId);

    public Task<bool> SendUserCreated(User user);

    public Task<bool> SendPromotionJoined(string userId);
}