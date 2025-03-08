using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;
using BookBoostApi.Interfaces;

namespace BookBoostApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AdController : ControllerBase
{
    private readonly ILogger<AdController> _logger;

    private IAdService _adService;

    public AdController(ILogger<AdController> logger, IAdService adService)
    {
        _logger = logger;
        _adService = adService;
    }
 
    [HttpPost]
    public async Task<IActionResult> CreateAd([FromBody] AdUpload ad)
    {
        try
        {
            var createdAd = await _adService.CreateAd(ad, "jmjordan");
            return Created(createdAd.Id.ToString(), createdAd);
        }
        catch(ConflictException)
        {
            return BadRequest("Ad already exists");
        }
    }

    [HttpGet]
    public async Task<IEnumerable<Ad>> GetAds()
    {
        return await _adService.GetAds("jmjordan");
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAd(string id)
    {
        var count = await _adService.DeleteAd("jmjordan", id);
        return count == 1 ? Ok() : NotFound();
    }

    [HttpPatch]
    public async Task<ActionResult> UpdateAd([FromBody] Ad ad)
    {
        var result = await _adService.UpdateAd(ad);
        return result != null ? Ok(result) : NotFound();
    }
}
