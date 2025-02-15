using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;
using BookBoostApi.Interfaces;
using System.Reflection.Metadata.Ecma335;

namespace BookBoostApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BookController : ControllerBase
{
    private readonly ILogger<BookController> _logger;

    private IBookService _bookService;

    public BookController(ILogger<BookController> logger, IBookService bookApi)
    {
        _logger = logger;
        _bookService = bookApi;
    }

    [HttpPost()]
    public bool UploadBook([FromBody] BookUpload book)
    {
        return _bookService.UploadBook(book);
    }

    [HttpGet("uploads")]
    public IEnumerable<BookUploadRecord> GetBookUploadRecords()
    {
        return _bookService.GetBookUploadRecords("jmjordan");
    }

    [HttpGet()]
    public IEnumerable<Book> GetBooks([FromBody] BookSearch bookSearch)
    {
        return _bookService.GetBooks(bookSearch);
    }
}
