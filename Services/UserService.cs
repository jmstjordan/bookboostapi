using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookBoostApi.Services;

public class UserService : IUserService
{
    private readonly IMongoCollection<User> _usersCollection;

    public UserService(IOptions<BookBoostDatabaseSettings> bookBoostDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            bookBoostDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookBoostDatabaseSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<User>(
            bookBoostDatabaseSettings.Value.UsersCollectionName);
    }
  
    public async Task<User> GetUser(string userId)
    {
        return await _usersCollection.Find(x => x.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task UpsertUser(User user)
    {
        var filter = Builders<User>.Filter.Eq(x => x.UserId, user.UserId);
        await _usersCollection.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = true });
    }
}