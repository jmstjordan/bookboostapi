
namespace BookBoostApi.Interfaces;

public interface IRefreshTokenService
{
    public Task<RefreshToken> GetToken(RefreshRequest request);

    public Task RevokeToken(RefreshToken token);

    public Task AddToken(RefreshToken token);
}

