using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace BookBoostApi.Services;

public class StripeService : IPaymentService
{
    private string _apiKey;

    private IUserService _userService;

    private IAdService _adService;

    public StripeService(IOptions<PaymentSettings> settings, IUserService userService, IAdService adService)
    {
        _apiKey = settings.Value.StripeApiKey;
        _userService = userService;
        _adService = adService;
    }

    public async Task<string> CreateAdCheckoutSession(AdUpload ad, string requestHost, string userId, int price)
    {
        StripeConfiguration.ApiKey = _apiKey;

        var user = await _userService.GetUser(userId);
        if(user.CustomerId == null)
        {
            user.CustomerId = CreateStripeCustomer(userId).Id;
            await _userService.UpdateUserField(user.Id, "CustomerId", user.CustomerId);
        }

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            Customer = user.CustomerId,
            Mode = "setup",
            SuccessUrl = $"{requestHost}/success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{requestHost}/author",
            SetupIntentData = new SessionSetupIntentDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    { "setup_for", "future_invoice" }
                }
            },
            Metadata = new Dictionary<string, string>
            {
                { "ProductId", ad.ProductUpload.ProductId },
                { "AdDate", ad.AdDate.ToShortDateString() },
                { "Genre", ad.Genre.ToString() },
                { "ProductSource", ad.ProductUpload.ProductSource.ToString() },
            }
        };

        var service = new SessionService();
        var session = service.Create(options);
        return session.Id;
    }

    private Customer CreateStripeCustomer(string userId)
    {
        var customerOptions = new CustomerCreateOptions
        {
            Metadata = new Dictionary<string, string>
            {
                { "app_user_id", userId }
            }
        };
        var customerService = new CustomerService();
        return customerService.Create(customerOptions);
    }

    public async Task<bool> ChargeAd(Ad ad)
    {
        StripeConfiguration.ApiKey = _apiKey;

        var user = await _userService.GetUser(ad.UserId);
        var paymentIntentService = new PaymentIntentService();
        var intent = paymentIntentService.Create(new PaymentIntentCreateOptions
        {
            Amount = ad.Price,
            Currency = "usd",
            Customer = user.CustomerId,
            PaymentMethod = ad.PaymentMethodId,
            OffSession = true,
            Confirm = true,
            Metadata = new Dictionary<string, string>
            {
                { "userId", user.Id },
                { "orderId", ad.OrderId }
            }
        });
        await _adService.UpdateField(ad.Id, "PaymentIntentId", intent.Id);
        return intent.Status == "succeeded";
    }

    public async Task<string> VerifySession(string sessionId)
    {
        var service = new SessionService();
        var session = service.Get(sessionId);
        var setupIntent = new SetupIntentService().Get(session.SetupIntentId);
        // TODO: check setupIntent.status
        return setupIntent.PaymentMethodId;
    }
}