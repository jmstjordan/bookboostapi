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

    private IProductService _productService;

    private IAppEmailService _emailService;

    public AdController(ILogger<AdController> logger, IAdService adService, IProductService productService, IAppEmailService emailService)
    {
        _logger = logger;
        _adService = adService;
        _productService = productService;
        _emailService = emailService;
    }
 
    [HttpPost]
    public async Task<IActionResult> CreateAd([FromBody] AdUpload ad)
    {
        try
        {
            if(ad.AdDate == null || ad.Genre == null || ad.Tier == null || ad.ProductId == null)
            {
                return BadRequest("Ad requires AdDate, Genre, Tier, and ProductId fields");
            }
            var product = await _productService.GetProduct(ad.ProductId);
            if(product == null)
            {
                return BadRequest("Invalid Product Id");
            }
            var createdAd = await _adService.CreateAd(ad, "jmjordan");
            await _emailService.SendAdCreated("jmstjordan");
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
    public async Task<ActionResult> UpdateAd(string id, [FromBody] AdUpload ad)
    {
        // TODO: if admin, ad bypass
        var existingAd = await _adService.GetAd("jmjordan", id);
        if(existingAd == null)
        {
            return NotFound("Ad not found");
        }
        if(ad.Genre != null)
            existingAd.Genre = ad.Genre;
        if(ad.Tier != null)
            existingAd.Tier = ad.Tier;
        if(ad.AdDate != null)
            existingAd.AdDate = ad.AdDate;
        if(ad.ProductId != null)
            existingAd.ProductId = ad.ProductId;
        if(true && ad.State != null){
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

    [HttpGet("Available/{tier}")]
    public async Task<ActionResult> GetAvailableDates(Tier tier)
    {
        return Ok(await _adService.AvailableAdDates(tier));
    }
}
