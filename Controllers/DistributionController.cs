using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Interfaces;

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
        var subscriber = new Subscriber
        {
            Email = subscriberUpload.Email,
            SubscriberSource = subscriberUpload.SubscriberSource
        };
        await _subscriberService.AddSubscriber(subscriber);
        return NoContent();
    }
}
