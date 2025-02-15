

using BookBoostApi.Interfaces;
using BookBoostApi.Models;

public class ProductService
{
    private IAmazonProductService _amazonProductService;

    public ProductService(IAmazonProductService amazonProductService)
    {
        _amazonProductService = amazonProductService;
        // and any other products, apple, google, barns and noble, etc.
    }

    public async Task<Product> GetProduct(string id, ProductSource productSource)
    {
        switch (productSource)
        {
            case ProductSource.Amazon:
                return await _amazonProductService.GetProduct(id);
        }
        throw new NotImplementedException();
    } 
}