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

    public AdController(ILogger<AdController> logger, IAdService adService, IAppEmailService emailService)
    {
        _logger = logger;
        _adService = adService;
        _emailService = emailService;
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
        await _adService.UpdateField(id, "State", ad.State);
        var currentAd = await _adService.GetAd(id);
        if(currentAd.State == AdState.Accepted)
        {
            await _emailService.SendAdAccepted(currentAd.UserId);
        }
        else if(currentAd.State == AdState.Declined)
        {
            await _emailService.SendAdDeclined(currentAd.UserId);
        }        
        else if(currentAd.State == AdState.Canceled)
        {
            await _emailService.SendAdDeclined(currentAd.UserId);
        }
        return Ok(currentAd.State);
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