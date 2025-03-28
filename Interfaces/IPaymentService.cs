
using BookBoostApi.Models;

namespace BookBoostApi.Interfaces;

public interface IPaymentService
{
    public string CreateAdCheckoutSession(AdUpload ad, string requestHost);
}

