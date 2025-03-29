using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;
using BookBoostApi.Interfaces;

namespace BookBoostApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;

    private IProductService _productService;


    public ProductController(ILogger<ProductController> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    [HttpGet]
    public async Task<IEnumerable<Product>> GetProducts([FromQuery] ProductSearch productSearch)
    {
        return await _productService.GetProducts(productSearch);
    }

    [HttpPost]
    public async Task<IActionResult> GetProduct([FromBody] ProductUpload productUpload)
    {
        var result = await _productService.GetProduct(productUpload);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpGet("Sources")]
    public ActionResult GetProductSources()
    {
        return Ok(_productService.GetProductSources());
    }
}
