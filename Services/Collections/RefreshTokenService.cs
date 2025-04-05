
using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IMongoCollection<RefreshToken> _refreshTokenCollection;

    public RefreshTokenService(IOptions<BookBoostDatabaseSettings> bookBoostDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            bookBoostDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookBoostDatabaseSettings.Value.DatabaseName);

        _refreshTokenCollection = mongoDatabase.GetCollection<RefreshToken>(
            bookBoostDatabaseSettings.Value.RefreshTokensCollectionName);
    }

    public async Task AddToken(RefreshToken token)
    {
        await _refreshTokenCollection.InsertOneAsync(token);
    }

    public async Task<RefreshToken> GetToken(RefreshRequest request)
    {
        return await _refreshTokenCollection
            .Find(x => x.Token == request.RefreshToken && !x.IsRevoked)
            .FirstOrDefaultAsync();
    }

    public async Task RevokeToken(RefreshToken token)
    {
        token.IsRevoked = true;
        await _refreshTokenCollection.ReplaceOneAsync(x => x.Id == token.Id, token);
    }
}
