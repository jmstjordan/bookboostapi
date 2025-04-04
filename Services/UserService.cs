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

    public async Task CreatUser(User user)
    {
        await _usersCollection.InsertOneAsync(user);
    }

    public async Task<User> GetUser(string username)
    {
        return await _usersCollection.Find(x => x.Username == username).FirstOrDefaultAsync();
    }

    public async Task<User> GetUserbyEmail(string email)
    {
        return await _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
    }

    // public async Task UpsertUser(User user)
    // {
    //     var filter = Builders<User>.Filter.Eq(x => x.UserId, user.UserId);
    //     await _usersCollection.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = true });
    // }
}