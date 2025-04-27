using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;
using BookBoostApi.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BookBoostApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private IProductService _productService;

    private IPriceService _priceService;

    public ProductController(IProductService productService, IPriceService priceService)
    {
        _productService = productService;
        _priceService = priceService;
    }

    [HttpPost("Load")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> LoadProducts([FromBody] ProductSearch productSearch)
    {
        await _productService.LoadProducts(productSearch);
        return NoContent();
    }

    [HttpGet]
    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await _productService.GetProducts();
    }

    [HttpPost("Validate")]
    [Authorize]
    public async Task<IActionResult> ValidateProduct([FromBody] ProductValidate product)
    {
        var result = await _productService.GetProductFromSource(product);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpGet("Me")]
    [Authorize]
    public async Task<IActionResult> GetProductsByUser()
    {
        var userId = HttpContext.GetUserId();
        return Ok(await _productService.GetProductsByUser(userId));
    }

    [HttpGet("Sources")]
    [Authorize]
    public ActionResult GetProductSources()
    {
        return Ok(_productService.GetProductSources());
    }

    [HttpGet("Prices")]
    [Authorize]
    public IActionResult GetProductPrices()
    {
        return Ok(_priceService.GetProductPrices());
    }
}
