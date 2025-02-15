namespace BookBoostApi.Models;

public class BookSearch
{
    public required string? Title { get; set; }

    public required string? Author { get; set; }

    public List<Category>? Categories { get; set; }
}