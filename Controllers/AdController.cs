using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;
using BookBoostApi.Interfaces;
using Stripe.Checkout;
using Stripe;

namespace BookBoostApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdController : ControllerBase
{
    private readonly ILogger<AdController> _logger;

    private IAdService _adService;


    private IAppEmailService _emailService;

    public AdController(ILogger<AdController> logger, IAdService adService, IAppEmailService emailService)
    {
        _logger = logger;
        _adService = adService;
        _emailService = emailService;
    }
 
    [HttpPost]
    public async Task<IActionResult> CreateAd([FromBody] AdUpload ad)
    {
        try
        {
            var createdAd = await _adService.CreateAd(ad, "jmjordan");
            await _emailService.SendAdCreated("jmstjordan");
            return Created(createdAd.Id.ToString(), createdAd);
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

    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        _logger.LogInformation("Tester");
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAds()
    {
        return Ok(await _adService.GetAds("jmjordan"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAd(string id)
    {
        var result = await _adService.GetAd("jmjordan", id);
        if(result == null)
        {
            return NotFound("Ad not found");
        }
        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAd(string id)
    {
        var count = await _adService.DeleteAd("jmjordan", id);
        return count == 1 ? Ok() : NotFound();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> UpdateAd(string id, [FromBody] AdPatch ad)
    {
        var existingAd = await _adService.GetAd("jmjordan", id);
        if(existingAd == null)
        {
            return NotFound("Ad not found");
        }
        if(true){
            // if we are an admin
            if(ad.State == AdState.Accepted)
            {
                await _emailService.SendAdAccepted("jmjordan");
            }
            else if(ad.State == AdState.Declined)
            {
                await _emailService.SendAdDeclined("jmjordan");
            }
            existingAd.State = ad.State;
        }
        var result = await _adService.UpdateAd(existingAd);
        return result != null ? Ok(result) : BadRequest();
    }

    [HttpGet("Availability/{genre}")]
    public async Task<ActionResult> GetAvailableDates(Genre genre)
    {
        return Ok(await _adService.AvailableAdDates(genre));
    }

    [HttpPost("create-checkout-session")]
    public IActionResult CreateCheckoutSession([FromBody] AdUpload request)
    {
        StripeConfiguration.ApiKey = "sk_test_mvmU9s1OfCaJyRM5n32JncRk";
        try
        {
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
                            UnitAmount = 5000, // $50.00 Example
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Product Example"
                            }
                        },
                        Quantity = 1,
                    }
                },
                Mode = "payment",
                SuccessUrl = "http://localhost:4280/home/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "http://localhost:4280/home/author",
                Metadata = new Dictionary<string, string>
                {
                    // { "OrderId", request.OrderId },
                    // { "CustomNote", request.CustomNote }
                }
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Ok(new { sessionId = session.Id });
        }
        catch (StripeException ex)
        {
            return BadRequest(new { error = ex.StripeError.Message });
        }
    }

    [HttpGet("verify-session/{sessionId}")]
    public IActionResult VerifySession(string sessionId)
    {
        try
        {
            var service = new SessionService();
            var session = service.Get(sessionId);

            // Check if the payment was successful
            if (session.PaymentStatus == "paid")
            {
                return Ok(new { paymentStatus = "succeeded" });
            }
            else
            {
                return Ok(new { paymentStatus = "failed" });
            }
        }
        catch (StripeException ex)
        {
            return BadRequest(new { error = ex.StripeError.Message });
        }
    }
}