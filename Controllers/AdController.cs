using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;
using BookBoostApi.Interfaces;
using System.ComponentModel;

namespace BookBoostApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AdController : ControllerBase
{
    private readonly ILogger<AdController> _logger;

    private IAdService _adService;

    private IProductService _productService;

    public AdController(ILogger<AdController> logger, IAdService adService, IProductService productService)
    {
        _logger = logger;
        _adService = adService;
        _productService = productService;
    }
 
    [HttpPost]
    public async Task<IActionResult> CreateAd([FromBody] AdUpload ad)
    {
        try
        {
            var product = await _productService.GetProduct(ad.ProductId);
            if(product == null)
            {
                return BadRequest("Invalid Product Id");
            }
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

    [HttpGet("{id}")]
    public async Task<Ad> GetAd(string id)
    {
        return await _adService.GetAd("jmjordan", id);
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
        var existingAd = await _adService.GetAd("jmjordan", id);
        if(existingAd == null)
        {
            return NotFound();
        }
        existingAd.Genre = ad.Genre;
        existingAd.Tier = ad.Tier;
        existingAd.AdDate = ad.AdDate;
        var result = await _adService.UpdateAd(existingAd);
        return result != null ? Ok(result) : BadRequest();
    }
}
