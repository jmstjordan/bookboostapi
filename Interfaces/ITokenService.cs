
namespace BookBoostApi.Interfaces;
using BookBoostApi.Models;

public interface ITokenService
{
    public Task<TokenStore> GetToken(string token);

    public Task RevokeToken(TokenStore token);

    public Task AddToken(TokenStore token);
}

