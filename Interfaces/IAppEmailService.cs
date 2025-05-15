using BookBoostApi.Models;

namespace BookBoostApi.Interfaces;

public interface IAppEmailService
{
    public Task<bool> SendAdCreated(string userId, Ad ad);

    public Task<bool> SendAdAccepted(string userId, Ad ad);

    public Task<bool> SendAdDeclined(string userId, Ad ad);

    public Task<bool> SendAdCanceled(string userId, Ad ad);

    public Task<bool> SendUserCreated(User user, Role role);

    public Task<bool> SendPromotionJoined(string userId);

    public Task<bool> SendPasswordReset(User user, string resetUrl);
}