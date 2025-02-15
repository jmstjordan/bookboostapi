namespace BookBoostApi.Models;

public class BookUpload
{
    public string Id { get; set; }

    public ProductSource ProductSource { get; set; }
}

public class BookLink
{
    public string ProductId { get; set;}

    public string Url { get; set;}

    public ProductSource ProductSource { get; set;}
}

public class BookUploadRecord
{
    public DateOnly UploadDate { get; set; }

    public string User { get; set; }

    public List<BookLink> BookLinks { get; set; }
}