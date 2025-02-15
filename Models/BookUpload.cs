namespace BookBoostApi.Models;

public class BookUpload
{
    public List<string> Urls { get; set; } = new List<string>();
}

public class BookLink
{
    public string ProductId { get; set;}

    public string Url { get; set;}

    public BookSource BookSource { get; set;}
}

public class BookUploadRecord
{
    public DateOnly UploadDate { get; set; }

    public string User { get; set; }

    public List<BookLink> BookLinks { get; set; }
}

public enum BookSource
{
    Amazon
}