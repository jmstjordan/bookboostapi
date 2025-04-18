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

    public AdController(ILogger<AdController> logger, IAdService adService, IAppEmailService emailService, IPaymentService paymentService)
    {
        _logger = logger;
        _adService = adService;
        _emailService = emailService;
        _paymentService = paymentService;
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
        if(currentAd.State == AdState.Accepted)
        {
            return BadRequest("This ad is already accepted");
        }
        await _adService.UpdateField(id, "State", ad.State);

        // retrieving ad one more time for safety and I'm too lazy to update my update function
        currentAd = await _adService.GetAd(id);
        if(currentAd.State == AdState.Accepted)
        {
            await _paymentService.ChargeAd(currentAd);
            await _emailService.SendAdAccepted(currentAd.UserId, currentAd);
        }
        else if(currentAd.State == AdState.Declined)
        {
            await _emailService.SendAdDeclined(currentAd.UserId, currentAd);
        }        
        else if(currentAd.State == AdState.Canceled)
        {
            await _emailService.SendAdDeclined(currentAd.UserId, currentAd);
        }
        return Ok(currentAd.State);
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
}