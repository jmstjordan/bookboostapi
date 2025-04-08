namespace BookBoostApi.Models;

public enum ProductSource
{
    Amazon = 1
}

public class Product
{
    public required string ProductId { get; set; }   

    public required ProductSource ProductSource { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public string? DescriptionView { get; set; }

    public string? Link { get; set; }

    public Price? Price { get; set; }

    public double Rating { get; set; }

    public int NumReviews { get; set; }

    public string? Image { get; set; }

    public string GetCacheKey()
    {
        return this.ProductId + this.ProductSource;
    }
}

public class ProductUpload
{
    public required string ProductId { get; set; }

    public required ProductSource ProductSource { get; set; }

    public string GetCacheKey()
    {
        return this.ProductId + this.ProductSource;
    }
}

public class ProductSearch
{
    // amazon kindle
    public required string CategoryId { get; set; } = "133140011";

    // TODO: change this to ENUM
    public string? SortBy { get; set; }

    public string? SearchTerm { get; set; }

    public string GetCacheKey()
    {
        return $"{this.CategoryId}_{this.SortBy}_{this.SearchTerm}";
    }
}