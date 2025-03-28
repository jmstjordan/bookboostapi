using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace BookBoostApi.Services;

public class StripeService : IPaymentService
{
    private string _apiKey;

    private IPriceService _priceService;

    public StripeService(IOptions<PaymentSettings> settings, IPriceService priceService)
    {
        _apiKey = settings.Value.StripeApiKey;
        _priceService = priceService;
    }

    public string CreateAdCheckoutSession(AdUpload ad, string requestHost)
    {
        StripeConfiguration.ApiKey = _apiKey;
        var prices = _priceService.GetPrices();
        var adPrice = prices[ad.Genre];

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = adPrice,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Product Example"
                        }
                    },
                    Quantity = 1,
                }
            },
            Mode = "payment",
            SuccessUrl = $"{requestHost}/home/success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{requestHost}/home/author",
            Metadata = new Dictionary<string, string>
            {
                { "ProductId", ad.ProductUpload.ProductId },
                { "AdDate", ad.AdDate.ToShortDateString() },
                { "Genre", ad.Genre.ToString() },
                { "ProductSource", ad.ProductUpload.ProductSource.ToString() }
            }
        };

        var service = new SessionService();
        var session = service.Create(options);
        return session.Id;
    }
}