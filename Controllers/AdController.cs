using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;
using BookBoostApi.Interfaces;

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
 
    // [HttpPost]
    // public async Task<IActionResult> CreateAd([FromBody] AdUpload ad)
    // {
    //     try
    //     {
    //         var createdAd = await _adService.CreateAd(ad, "jmjordan");
    //         await _emailService.SendAdCreated("jmstjordan");
    //         return Created(createdAd.Id.ToString(), createdAd);
    //     }
    //     catch(NotImplementedException e)
    //     {
    //         return BadRequest(e.Message);
    //     }
    //     catch(ConflictException e)
    //     {
    //         return BadRequest(e.Message);
    //     }
    //     catch(Exception e)
    //     {
    //         _logger.LogError(e.StackTrace);
    //         return StatusCode(500);
    //     }
    // }

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
}