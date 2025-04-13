using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
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

    public async Task<User> GetUser(string userId)
    {
        return await _usersCollection.Find(x => x.Id == userId).FirstOrDefaultAsync();
    }

    public async Task<User> GetUserbyEmail(string email)
    {
        return await _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
    }

    public async Task UpdatePassword(string userId, string passwordHash)
    {
        var update = Builders<User>.Update.Set("PasswordHash", passwordHash);

        await _usersCollection.UpdateOneAsync(
            Builders<User>.Filter.Eq("_id", ObjectId.Parse(userId)),
            update
        );
    }

    public async Task<bool> UpdatePreferences(string userId, Preferences preferences)
    {
        var update = Builders<User>.Update.Set("Preferences", preferences);

        var result = await _usersCollection.UpdateOneAsync(
            Builders<User>.Filter.Eq("_id", ObjectId.Parse(userId)),
            update
        );
        return result.ModifiedCount > 0;
    }

    public async Task UpdateRole(string userId, Role role)
    {
        var update = Builders<User>.Update.Set("Role", role);

        await _usersCollection.UpdateOneAsync(
            Builders<User>.Filter.Eq("_id", ObjectId.Parse(userId)),
            update
        );
    }

    public async Task UpdateUserField(string userId, string key, dynamic field)
    {
        var update = Builders<User>.Update.Set(key, field);

        await _usersCollection.UpdateOneAsync(
            Builders<User>.Filter.Eq("_id", ObjectId.Parse(userId)),
            update
        );
    }
}