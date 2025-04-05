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


    public PaymentController(ILogger<AdController> logger, IAdService adService, IAppEmailService emailService, IPriceService priceService, IPaymentService paymentService)
    {
        _logger = logger;
        _adService = adService;
        _emailService = emailService;
        _priceService = priceService;
        _paymentService = paymentService;
    }

    [HttpPost("CreateAdCheckoutSession")]
    [Authorize]
    public async Task<IActionResult> CreateAdCheckoutSession([FromBody] AdUpload ad)
    {
        var requestHost = HttpContext.GetRequestHost();
        try
        {
            var sessionId = _paymentService.CreateAdCheckoutSession(ad, requestHost);
            await _adService.CreateAd(ad, sessionId, HttpContext.GetUserId());
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
    [Authorize]
    public async Task<IActionResult> VerifySession(string sessionId)
    {
        var userId = HttpContext.GetUserId();
        var result = _paymentService.VerifySession(sessionId);
        if(result)
        {
            var confirmed = await _adService.ConfirmPaymentAd(sessionId, userId);
            if(confirmed)
            {
                await _emailService.SendAdCreated(userId);
            }
            var ad = await _adService.GetAdBySessionId(userId, sessionId);
            return Ok(ad);
        }
        return NoContent();
    }

    [HttpGet("Prices")]
    [Authorize]
    public IActionResult GetPrices()
    {
        return Ok(_priceService.GetPrices());
    }
}