
using BookBoostApi.Models;

namespace BookBoostApi.Interfaces;

public interface IPaymentService
{
    public Task<string> CreateAdCheckoutSession(AdUpload ad, string requestHost, string userId, int price);

    public Task<string> VerifySession(string sessionId);

    public Task<bool> ChargeAd(Ad ad);
}

