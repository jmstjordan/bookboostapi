namespace BookBoostApi.Models;

public class BookView
{
    public DateOnly ListDate { get; set; }

    public required string Title { get; set; }

    public required string Author { get; set; }

    public double Rating { get; set; }

    public int NumReviews { get; set; }

    public string? SourceImageUrl { get; set; }

    public string? Summary { get; set; }

    public string? Description { get; set; }

    public int PageLength { get; set; }

    public List<Category> Categories { get; set; } = new List<Category>();
}
