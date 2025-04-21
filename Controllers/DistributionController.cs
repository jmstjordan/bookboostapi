using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Interfaces;
using System.ComponentModel.DataAnnotations;

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
        await _subscriberService.AddSubscriber(subscriber);
        return NoContent();
    }
}
