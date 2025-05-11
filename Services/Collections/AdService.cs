namespace BookBoostApi.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

public class AdService : IAdService
{
    private readonly IMongoCollection<Ad> _adsCollection;
    private IProductService _productService;
    private const int MIN_DAYS = 30;
    private const int LENGTH_OF_AD_CALENDER = 60;
    private const int MAX_AD_PER_DAY = 5;

    public AdService(IOptions<BookBoostDatabaseSettings> bookBoostDatabaseSettings, IProductService productService)
    {
        // _amazonProductService = amazonProductService;
        // // and any other products, apple, google, barns and noble, etc.
        _productService = productService;

        var mongoClient = new MongoClient(
            bookBoostDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookBoostDatabaseSettings.Value.DatabaseName);

        _adsCollection = mongoDatabase.GetCollection<Ad>(
            bookBoostDatabaseSettings.Value.AdsCollectionName);
    }

    public async Task<Ad> CreateAd(AdUpload ad, string sessionId, string userId, int price, Product product)
    {
        if(AdExistsByUser(userId, product, ad.AdDate, ad.Genre))
        {
            throw new ConflictException("Ad already exists");
        }
        if(!await AdDateEligible(ad.Genre, ad.AdDate))
        {
            throw new ConflictException("Ad Date not available");
        }
        var newAd = new Ad 
        {
            Genre = ad.Genre,
            ProductId = product.Id,
            AdDate = ad.AdDate,
            UserId = userId,
            State = AdState.Pending,
            SessionId = sessionId,
            Price = price,
        };
        await _adsCollection.InsertOneAsync(newAd);
        return newAd; 
    }

    private async Task<bool> AdDateEligible(Genre genre, DateOnly adDate)
    {
        var dates = await AvailableAdDates(genre);
        foreach(AdAvailability date in dates)
        {
            if(DateOnly.Parse(date.AdDate) == adDate)
            {
                return date.Count > 0;
            }
        }
        return false;
    }

    private async Task<DateOnly> AssignAdDate(Genre genre)
    {
        var dates = await AvailableAdDates(genre);
        foreach(AdAvailability date in dates)
        {
            if(date.Count > 0)
            {
                return DateOnly.Parse(date.AdDate);
            }
        }
        throw new Exception("Unable to find Ad Date");
    }

    private bool AdExistsByUser(string userId, Product product, DateOnly adDate, Genre genre)
    {
        return _adsCollection.CountDocuments(x => x.ProductId == product.Id 
            && x.UserId == userId 
            && x.Genre == genre
            && x.AdDate == adDate
        ) > 0;
    }

    public IEnumerable<Genre> GetGenres()
    {
        return (Genre[])Enum.GetValues(typeof(Genre));
    }

    public async Task<IEnumerable<Ad>> GetAds(string userId)
    {
        return await _adsCollection.Find(x => x.UserId == userId).ToListAsync();
    }

    public async Task<Ad> GetAd(string adId)
    {
        return await _adsCollection.Find(x => x.Id == adId).FirstOrDefaultAsync();
    }

    public async Task<Ad> GetAdBySessionId(string userId, string sessionId)
    {
        return await _adsCollection.Find(x => x.UserId == userId && x.SessionId == sessionId).FirstOrDefaultAsync();
    }

    public async Task<bool> ConfirmPaymentAd(string sessionId, string userId)
    {
        var filter = Builders<Ad>.Filter.And(
            Builders<Ad>.Filter.Eq("SessionId", sessionId),
            Builders<Ad>.Filter.Eq(x => x.UserId, userId)
        );
        var update = Builders<Ad>.Update.Set("PaymentScheduled", true);

        var result = await _adsCollection.UpdateOneAsync(filter, update);
        return result.ModifiedCount == 1;
    }

    public async Task<IEnumerable<AdAvailability>> AvailableAdDates(Genre genre)
    {
        DateTime startDate;
        DateTime endDate;
        if(DateTime.Today < new DateTime(2025, 8, 1))
        {
            startDate = new DateTime(2025, 9, 1);
            endDate = startDate.AddDays(LENGTH_OF_AD_CALENDER);
        }
        else
        {
            startDate = DateTime.UtcNow.AddDays(MIN_DAYS);
            endDate = startDate.AddDays(LENGTH_OF_AD_CALENDER);
        }
        var allDates = Enumerable.Range(0, (endDate - startDate).Days)
                                 .Select(offset => startDate.AddDays(offset).ToString("yyyy-MM-dd"))
                                 .ToList();

        var pipeline = new[]
        {
            // Step 1: Extract date (ignoring time) and keep the field we want to group by
            new BsonDocument("$project", new BsonDocument
            {
                { "dateOnly", new BsonDocument("$dateToString", new BsonDocument
                    {
                        { "format", "%Y-%m-%d" },
                        { "date", "$AdDate" }
                    })
                },
                { "categoryField", "$Genre" }  // Replace 'yourField' with the field you want to bucket by
            }),

            // Step 2: Match documents within the date range
            new BsonDocument("$match", new BsonDocument
            {
                { "dateOnly", new BsonDocument
                    {
                        { "$gte", startDate.ToString("yyyy-MM-dd") },
                        { "$lte", endDate.ToString("yyyy-MM-dd") }
                    }
                }
            }),

            // Step 3: Group by date and category field, count occurrences
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", new BsonDocument
                    {
                        { "date", "$dateOnly" },
                        { "category", "$categoryField" }
                    }
                },
                { "count", new BsonDocument("$sum", 1) }
            }),

            // Step 4: Group again to structure the output as { date: ..., counts: [{category, count}, ...] }
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$_id.date" },
                { "counts", new BsonDocument("$push", new BsonDocument
                    {
                        { "category", "$_id.category" },
                        { "count", "$count" }
                    })
                }
            }),

            // Step 5: Sort results by date
            new BsonDocument("$sort", new BsonDocument
            {
                { "_id", 1 }
            })
        };

        var results = await _adsCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();

        var processedResults = allDates.Select(date =>
        {
            var result = results.FirstOrDefault(r => r["_id"] == date);
            return new BsonDocument
            {
                { "date", date },
                { "counts", result != null ? result["counts"].AsBsonArray : new BsonArray() }
            };
        }).ToList();

        // Step 7: Print results
        var adDates = new List<AdAvailability>();
        foreach (var result in processedResults)
        {
            foreach (var category in result["counts"].AsBsonArray)
            {
                if(category["category"].AsString == genre.ToString())
                {
                    adDates.Add(new AdAvailability { AdDate = result["date"].AsString, Count = category["count"].AsInt32} );
                }
            }
        }

        var res = allDates.Select(date =>
        {
            var result = adDates.FirstOrDefault(r => r.AdDate == date);
            var count = result != null ? MAX_AD_PER_DAY - result.Count : MAX_AD_PER_DAY;
            return new AdAvailability
            {
                AdDate = date,
                Count = count
            };
        }).ToList();
        return res;
    }

    public async Task UpdateField(string adId, string key, dynamic field)
    {
        var update = Builders<Ad>.Update.Set(key, field);

        await _adsCollection.UpdateOneAsync(
            Builders<Ad>.Filter.Eq("_id", ObjectId.Parse(adId)),
            update
        );
    }
}