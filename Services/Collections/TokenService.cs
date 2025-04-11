
using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class TokenService : ITokenService
{
    private readonly IMongoCollection<TokenStore> _tokenCollection;

    public TokenService(IOptions<BookBoostDatabaseSettings> bookBoostDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            bookBoostDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookBoostDatabaseSettings.Value.DatabaseName);

        _tokenCollection = mongoDatabase.GetCollection<TokenStore>(
            bookBoostDatabaseSettings.Value.TokensCollectionName);
    }

    public async Task AddToken(TokenStore token)
    {
        await _tokenCollection.InsertOneAsync(token);
    }

    public async Task<TokenStore> GetToken(string token)
    {
        return await _tokenCollection
            .Find(x => x.Token == token && !x.IsRevoked)
            .FirstOrDefaultAsync();
    }

    public async Task RevokeToken(TokenStore token)
    {
        token.IsRevoked = true;
        await _tokenCollection.ReplaceOneAsync(x => x.Id == token.Id, token);
    }
}
