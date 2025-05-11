using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;
using BookBoostApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace BookBoostApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private IProductService _productService;

    private IPriceService _priceService;

    public ProductController(IProductService productService, IPriceService priceService, ILogger<ProductController> logger)
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
        var activity = Activity.Current;
        if(activity != null)
        {
            activity.SetTag("ProductId", product.ProductId);
        }
        var result = await _productService.GetProductFromSource(product);
        if(result != null)
        {
            result.UserId = HttpContext.GetUserId();
            await _productService.AddProduct(result);
            return Ok(result);
        }
        return NotFound();
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
