
using BookBoostApi.Models;
namespace BookBoostApi.Interfaces;

public interface IAdService
{
    public Task<Ad> CreateAd(AdUpload ad, string sessionId, string user);

    public Task<IEnumerable<Ad>> GetAds(string userId);

    public Task<Ad> GetAd(string userId, string adId);

    public Task<Ad> UpdateAd(Ad ad);

    public Task<long> DeleteAd(string userId, string id);

    public Task<IEnumerable<AdAvailability>> AvailableAdDates(Genre genre);

    public Task<bool> ConfirmPaymentAd(string sessionId, string user);

    public Task<Ad> GetAdBySessionId(string user, string sessionId);

    public IEnumerable<Genre> GetGenres();

}

