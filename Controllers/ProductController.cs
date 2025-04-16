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

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost("Load")]
    // [Authorize(Roles = "admin")]
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

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> GetProduct([FromBody] ProductUpload productUpload)
    {
        var result = await _productService.GetProduct(productUpload);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpGet("Sources")]
    [Authorize]
    public ActionResult GetProductSources()
    {
        return Ok(_productService.GetProductSources());
    }
}
