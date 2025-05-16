namespace BookBoostApi.Services;

using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

public class SubscriberService : ISubscriberService
{
    private readonly IMongoCollection<Subscriber> _subscriberCollection;

    private IUserService _userService;
    
    public SubscriberService(IOptions<BookBoostDatabaseSettings> settings, IUserService userService)
    {
        var mongoClient = new MongoClient(
            settings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            settings.Value.DatabaseName);

        _subscriberCollection = mongoDatabase.GetCollection<Subscriber>(
            settings.Value.SubscribersCollectionName);

        _userService = userService;
    }

    public async Task<Subscriber> UpsertSubscriber(Subscriber subscriber)
    {
        var filter = Builders<Subscriber>.Filter.Eq(s => s.Email, subscriber.Email);

        var update = Builders<Subscriber>.Update
            .Set(u => u.IsSubscribed, subscriber.IsSubscribed)
            .Set(u => u.Topics, subscriber.Topics)
            .Set(u => u.SubscriberSource, subscriber.SubscriberSource)
            .SetOnInsert(u => u.Created, DateTime.UtcNow); // Only set Created if inserting

        var options = new FindOneAndUpdateOptions<Subscriber>
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After // Return the updated/inserted document
        };

        var result = await _subscriberCollection.FindOneAndUpdateAsync(filter, update, options);
        return result;
    }

    public async Task AddSubscriber(Subscriber subscriber)
    {
        await _subscriberCollection.InsertOneAsync(subscriber);
    }

    public async Task<Subscriber> GetSubscriberByUserId(string userId)
    {
        var user = await _userService.GetUser(userId);
        return await _subscriberCollection.Find(x => x.Id == user.SubscriberId).FirstOrDefaultAsync();
    }

    public async Task Subscribe(string userId, bool sub)
    {
        var user = await _userService.GetUser(userId);
        var update = Builders<Subscriber>.Update.Set("IsSubscribed", sub);
        await _subscriberCollection.UpdateOneAsync(
            Builders<Subscriber>.Filter.Eq("_id", ObjectId.Parse(user.SubscriberId)),
            update
        );
    }

    public async Task<Subscriber> UpdateSubscriber(string userId, SubscriberPatch subscriber)
    {
        var user = await _userService.GetUser(userId);
        var filter = Builders<Subscriber>.Filter.Eq(s => s.Id, user.SubscriberId);

        var update = Builders<Subscriber>.Update
            .Set(u => u.IsSubscribed, subscriber.IsSubscribed)
            .Set(u => u.Topics, subscriber.Topics);

        var options = new FindOneAndUpdateOptions<Subscriber>
        {
            ReturnDocument = ReturnDocument.After // Return the updated
        };

        return await _subscriberCollection.FindOneAndUpdateAsync(filter, update, options);
    }

    public async Task<Subscriber> GetSubscriberByEmail(string email)
    {
        return await _subscriberCollection.Find(s => s.Email == email).FirstOrDefaultAsync();
    }
}