
using BookBoostApi.Models;
namespace BookBoostApi.Interfaces;

public interface IAuthService
{
    public string GenerateAccessToken(User user);
    public Task<TokenStore> GenerateToken(User user, TokenType tokenType, int expirationHours);
}

