
using BookBoostApi.Models;
namespace BookBoostApi.Interfaces;

public interface IAdService
{
    public Task<Ad> CreateAd(AdUpload ad, string user);

    public Task<IEnumerable<Ad>> GetAds(string user);

    public Task<Ad> GetAd(string user, string adId);

    public Task<Ad> UpdateAd(Ad ad);

    public Task<long> DeleteAd(string user, string id);

    public Task<IEnumerable<AdAvailability>> AvailableAdDates(Tier tier);

}

