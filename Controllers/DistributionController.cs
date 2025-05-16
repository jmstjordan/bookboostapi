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
        var subscriber = await _subscriberService.GetSubscriberByEmail(subscriberUpload.Email);
        if(subscriber == null)
        {
            subscriber = new Subscriber
            {
                Email = subscriberUpload.Email,
                SubscriberSource = subscriberUpload.SubscriberSource
            };
        }
        else
        {
            return Conflict("Email address already exists");
        }
        await _subscriberService.AddSubscriber(subscriber);
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

    [HttpPatch("Subscriber")]
    [Authorize]
    public async Task<IActionResult> UpdateSubscriber([FromBody] SubscriberPatch subscriber)
    {
        var userId = HttpContext.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }
        var updated = await _subscriberService.UpdateSubscriber(userId, subscriber);
        return Ok(updated);
    }
}
