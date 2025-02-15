using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;
using BookBoostApi.Interfaces;

namespace BookBoostApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BooksController : ControllerBase
{
    private readonly ILogger<BooksController> _logger;

    private IBookService _bookService;

    public BooksController(ILogger<BooksController> logger, IBookService bookApi)
    {
        _logger = logger;
        _bookService = bookApi;
    }

    [HttpPost]
    public async Task UploadBook([FromBody] IEnumerable<BookUpload> books)
    {
        await _bookService.UploadBook(books);
    }

    [HttpGet("uploads")]
    public IEnumerable<BookUploadRecord> GetBookUploadRecords()
    {
        return _bookService.GetBookUploadRecords("jmjordan");
    }

    [HttpGet]
    public IEnumerable<Book> GetBooks([FromQuery] BookSearch bookSearch)
    {
        return _bookService.GetBooks(bookSearch);
    }
}
