using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;
using BookBoostApi.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BookBoostApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdController : ControllerBase
{
    private readonly ILogger<AdController> _logger;

    private IAdService _adService;

    private IAppEmailService _emailService;

    private IPaymentService _paymentService;

    private IPriceService _priceService;

    public AdController(ILogger<AdController> logger, IAdService adService, IAppEmailService emailService, IPaymentService paymentService, IPriceService priceService)
    {
        _logger = logger;
        _adService = adService;
        _emailService = emailService;
        _paymentService = paymentService;
        _priceService = priceService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAds()
    {
        return Ok(await _adService.GetAds(HttpContext.GetUserId()));
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult> UpdateAd(string id, [FromBody] AdPatch ad)
    {
        // only supports state change for now
        var currentAd = await _adService.GetAd(id);
        if(currentAd.State != AdState.Pending)
        {
            return BadRequest("This ad is not in a pending state. Please create a new one.");
        }
        if(ad.State == AdState.Accepted)
        {
            if(await _paymentService.ChargeAd(currentAd))
            {
                await _emailService.SendAdAccepted(currentAd.UserId, currentAd);
            }
            else
            {
                await _emailService.SendPaymentIssue(currentAd.UserId, currentAd);
                ad.State = AdState.Canceled;
                return BadRequest("Unable to charge user for ad");
            }
        }
        else if(ad.State == AdState.Declined)
        {
            await _emailService.SendAdDeclined(currentAd.UserId, currentAd);
        }        
        else if(ad.State == AdState.Canceled)
        {
            await _emailService.SendAdCanceled(currentAd.UserId, currentAd);
        }
        await _adService.UpdateField(id, "State", ad.State);
        return Ok(ad.State);
    }

    [HttpPost("{id}/Cancel")]
    [Authorize]
    public async Task<ActionResult> CancelAd(string id)
    {
        var userId = HttpContext.GetUserId();
        var ad = await _adService.GetAd(id);
        if(ad == null || ad.UserId != userId)
        {
            return NotFound();
        }
        if(ad.State == AdState.Canceled)
        {
            return BadRequest("This ad is already canceled");
        }
        await _adService.UpdateField(ad.Id, "State", AdState.Canceled);
        await _emailService.SendAdCanceled(userId, ad);
        return NoContent();
    }

    [HttpGet("Availability/{genre}")]
    [Authorize]
    public async Task<ActionResult> GetAvailableDates(Genre genre)
    {
        return Ok(await _adService.AvailableAdDates(genre));
    }

    [HttpGet("Genres")]
    [Authorize]
    public ActionResult GetGenres()
    {
        return Ok(_adService.GetGenres());
    }

    [HttpGet("Prices")]
    [Authorize]
    public IActionResult GetAdPrices()
    {
        return Ok(_priceService.GetAdPrices());
    }
}