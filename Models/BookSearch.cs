namespace BookBoostApi.Models;

public class BookSearch : IEquatable<BookSearch>
{
    public required string? Title { get; set; }

    public required string? Author { get; set; }

    public List<Category>? Categories { get; set; }

    public bool Equals(BookSearch? other)
    {
        return true;
    }
}