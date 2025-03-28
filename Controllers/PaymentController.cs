using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;
using BookBoostApi.Interfaces;
using Stripe.Checkout;
using Stripe;

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


    public PaymentController(ILogger<AdController> logger, IAdService adService, IAppEmailService emailService, IPriceService priceService, IPaymentService paymentService)
    {
        _logger = logger;
        _adService = adService;
        _emailService = emailService;
        _priceService = priceService;
        _paymentService = paymentService;
    }

    [HttpPost("CreateAdCheckoutSession")]
    public async Task<IActionResult> CreateAdCheckoutSession([FromBody] AdUpload ad)
    {
        var requestHost = $"{HttpContext.Request.Headers["Origin"].FirstOrDefault()}";
        try
        {
            var sessionId = _paymentService.CreateAdCheckoutSession(ad, requestHost);
            await _adService.CreateAd(ad, sessionId, "jmjordan");
            return Ok(new { sessionId });
        }
        catch (StripeException e)
        {
            _logger.LogError(e.StripeError.Message);
            return StatusCode(500);
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
    public async Task<IActionResult> VerifySession(string sessionId)
    {
        try
        {
            var service = new SessionService();
            var session = service.Get(sessionId);

            // Check if the payment was successful
            if (session.PaymentStatus == "paid")
            {
                var confirmed = await _adService.ConfirmPaymentAd(sessionId, "jmjordan");
                if(confirmed)
                {
                    await _emailService.SendAdCreated("jmstjordan");
                }
                return Ok(new {paymentSucceeded = true});
            }
            else
            {
                return Ok(new {paymentSucceeded = false});
            }
        }
        catch (StripeException e)
        {
            return BadRequest(e.StripeError.Message);
        }
    }

    [HttpGet("Prices")]
    public IActionResult GetPrices()
    {
        return Ok(_priceService.GetPrices());
    }
}