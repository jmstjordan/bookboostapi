namespace BookBoostApi.Services;

using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class AdService : IAdService
{
    private readonly IMongoCollection<Ad> _adsCollection;

    public AdService(IOptions<BookBoostDatabaseSettings> bookBoostDatabaseSettings)
    {
        // _amazonProductService = amazonProductService;
        // // and any other products, apple, google, barns and noble, etc.

        var mongoClient = new MongoClient(
            bookBoostDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookBoostDatabaseSettings.Value.DatabaseName);

        _adsCollection = mongoDatabase.GetCollection<Ad>(
            bookBoostDatabaseSettings.Value.AdsCollectionName);
    }

    public async Task<Ad> CreateAd(AdUpload ad, string user)
    {
        if(AdExistsByUser("jmjordan", ad.ProductId, ad.AdDate, ad.Genre))
        {
            throw new ConflictException("Ad already exists");
        }
        var newAd = new Ad 
        {
            Genre = ad.Genre,
            ProductId = ad.ProductId,
            AdDate = ad.AdDate,
            Tier = ad.Tier,
            User = user
        };
        await _adsCollection.InsertOneAsync(newAd);
        return newAd; 
    }

    private bool AdExistsByUser(string user, string productId, DateOnly adDate, Genre genre)
    {
        return _adsCollection.CountDocuments(x => x.ProductId == productId 
            && x.User == user 
            && x.Genre == genre
            && x.AdDate == adDate
        ) > 0;
    }

    public async Task<IEnumerable<Ad>> GetAds(string user)
    {
        return await _adsCollection.Find(x => x.User == user).ToListAsync();
    }

    public async Task<Ad> GetAd(string user, string adId)
    {
        return await _adsCollection.Find(x => x.User == user && x.Id == adId).FirstOrDefaultAsync();
    }

    public async Task<Ad> UpdateAd(Ad ad)
    {
        var result = await _adsCollection.ReplaceOneAsync(x => x.Id == ad.Id && x.User == ad.User, ad);
        if(result.MatchedCount == 1)
        {
            return await GetAd(ad.User, ad.Id.ToString());
        }
        return null;
    }

    public async Task<long> DeleteAd(string user, string id)
    {
        var result = await _adsCollection.DeleteOneAsync(x => x.Id == id && x.User == user);
        return result.DeletedCount;
    }
}