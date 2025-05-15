using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace BookBoostApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DistributionController : ControllerBase
{
    private ISubscriberService _subscriberService;

    public DistributionController(ISubscriberService subscriberService)
    {
        _subscriberService = subscriberService;
    }

    [HttpPost("Subscriber")]
    public async Task<IActionResult> AddSubscriber([FromBody] SubscriberUpload subscriberUpload)
    {
        if(!new EmailAddressAttribute().IsValid(subscriberUpload.Email))
        {
            return BadRequest("Invalid Email");
        }
        var subscriber = new Subscriber
        {
            Email = subscriberUpload.Email,
            SubscriberSource = subscriberUpload.SubscriberSource
        };
        await _subscriberService.UpsertSubscriber(subscriber);
        return NoContent();
    }

    [HttpGet("Subscriber")]
    public async Task<IActionResult> GetSubscriber()
    {
        var userId = HttpContext.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }
        return Ok(await _subscriberService.GetSubscriberByUserId(userId));
    }

    [HttpPatch("Subscriber/Preferences")]
    [Authorize]
    public async Task<IActionResult> UpdateSubscriberPreferences([FromBody] Preferences preferences)
    {
        var userId = HttpContext.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }
        var updated = await _subscriberService.UpdatePreferences(userId, preferences);
        return Ok(updated);
    }

    [HttpPatch("Subscriber/Subscribe")]
    [Authorize]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeUpload subscribe)
    {
        var userId = HttpContext.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }
        await _subscriberService.Subscribe(userId, subscribe.Subscribe);
        return Ok();
    }
}
