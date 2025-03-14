using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;
using BookBoostApi.Interfaces;

namespace BookBoostApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;

    private IProductService _productService;

    private IAiService _aiService;

    public ProductController(ILogger<ProductController> logger, IProductService productService, IAiService aiService)
    {
        _logger = logger;
        _productService = productService;
        _aiService = aiService;
    }

    // [HttpGet("ai")]
    public async Task<IActionResult> TestAi(string data)
    {
        return Ok(await _aiService.TrimDescription(data, 250));
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductUpload product)
    {
        try
        {
            var createdProject = await _productService.CreateProduct(product);
            return Created(createdProject.Id.ToString(), createdProject);
        }
        catch(ConflictException)
        {
            return BadRequest("Product already exists");
        }
        catch(NotImplementedException)
        {
            return BadRequest("Product Source not implemented");
        }
    }

    [HttpGet]
    public async Task<IEnumerable<Product>> GetProducts([FromQuery] ProductSearch productSearch)
    {
        return await _productService.GetProducts(productSearch);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(string id)
    {
        var result = await _productService.GetProduct(id);
        return result != null ? Ok(result) : NotFound();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        var count = await _productService.DeleteProduct("jmjordan", id);
        return count == 1 ? Ok() : NotFound();
    }
}
