
using BookBoostApi.Models;
namespace BookBoostApi.Interfaces;

public interface IAuthService
{
    public string GenerateAccessToken(User user);
    public Task<RefreshToken> GenerateRefreshToken(User user);
}

