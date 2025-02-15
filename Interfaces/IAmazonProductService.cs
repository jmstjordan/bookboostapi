
using BookBoostApi.Models;

namespace BookBoostApi.Interfaces;

public interface IAmazonProductService
{
    public Task<AmazonProduct> GetProduct(string id);
}

