namespace BookBoostApi.Services;

using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class SubscriberService : ISubscriberService
{
    private readonly IMongoCollection<Subscriber> _subscriberCollection;
    
    public SubscriberService(IOptions<BookBoostDatabaseSettings> settings)
    {
        var mongoClient = new MongoClient(
            settings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            settings.Value.DatabaseName);

        _subscriberCollection = mongoDatabase.GetCollection<Subscriber>(
            settings.Value.SubscribersCollectionName);
    }

    public async Task AddSubscriber(Subscriber subscriber)
    {
        var filter = Builders<Subscriber>.Filter.Eq(s => s.Email, subscriber.Email);

        var update = Builders<Subscriber>.Update
            .Set(u => u.IsSubscribed, subscriber.IsSubscribed)
            .SetOnInsert(u => u.Created, DateOnly.FromDateTime(DateTime.Now)); // Only set Created if inserting

        var options = new UpdateOptions { IsUpsert = true };
        await _subscriberCollection.UpdateOneAsync(filter, update, options);
    }
}