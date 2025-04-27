using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;
using BookBoostApi.Interfaces;
using Stripe;
using Microsoft.AspNetCore.Authorization;

namespace BookBoostApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly ILogger<AdController> _logger;

    private IAdService _adService;

    private IAppEmailService _emailService;

    private IPriceService _priceService;

    private IPaymentService _paymentService;

    private IProductService _productService;

    public PaymentController(ILogger<AdController> logger, IAdService adService, IAppEmailService emailService, IPriceService priceService, IPaymentService paymentService, IProductService productService)
    {
        _logger = logger;
        _adService = adService;
        _emailService = emailService;
        _priceService = priceService;
        _paymentService = paymentService;
        _productService = productService;
    }

    [HttpPost("CreateAdCheckoutSession")]
    [Authorize]
    public async Task<IActionResult> CreateAdCheckoutSession([FromBody] AdUpload ad)
    {
        var requestHost = HttpContext.GetRequestHost();
        try
        {
            var prices = _priceService.GetAdPrices();
            var adPrice = prices[ad.Genre];

            var productPrices = _priceService.GetProductPrices();
            if(!productPrices.Contains(ad.ProductUpload.OfferPrice))
            {
                return BadRequest("Offer price not listed");
            }

            var userId = HttpContext.GetUserId();
            var sessionId = await _paymentService.CreateAdCheckoutSession(ad, requestHost, userId, adPrice);
            var product = await _productService.CreateProduct(ad.ProductUpload, userId);
            if(product == null)
            {
                return BadRequest("Product unable to be created. ProductId likely not found");
            }
            var newAd = await _adService.CreateAd(ad, sessionId, userId, adPrice, product);
            return Ok(new { sessionId });
        }
        catch (StripeException e)
        {
            _logger.LogError(e.StripeError.Message);
            return StatusCode(500);
        }
        catch (ProductException e)
        {
            _logger.LogError(e.Message);
            return BadRequest(e.Message);
        }
        catch(NotImplementedException e)
        {
            return BadRequest(e.Message);
        }
        catch(ConflictException e)
        {
            return BadRequest(e.Message);
        }
        catch(Exception e)
        {
            _logger.LogError(e.StackTrace);
            return StatusCode(500);
        }
    }

    [HttpGet("VerifySession/{sessionId}")]
    [Authorize]
    public async Task<IActionResult> VerifySession(string sessionId)
    {
        var paymentMethodId = await _paymentService.VerifySession(sessionId);
        if(paymentMethodId == null)
        {
        return StatusCode(500, "Unable to retrieve PaymentId from Session");
        }
        var userId = HttpContext.GetUserId();
        var ad = await _adService.GetAdBySessionId(userId, sessionId);
        await _adService.UpdateField(ad.Id, "PaymentMethodId", paymentMethodId);
        await _emailService.SendAdCreated(userId, ad);

        return Ok(ad);
    }

    // [HttpGet("Charge/{adId}")]
    // [Authorize]
    // public async Task<IActionResult> ChargeCustomer(string adId)
    // {
    //     var ad = await _adService.GetAd(adId);
    //     return Ok(await _paymentService.ChargeAd(ad));
    // }
}