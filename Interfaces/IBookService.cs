
using BookBoostApi.Models;

namespace BookBoostApi.Interfaces
{
    public interface IBookService
    {
        public IEnumerable<Book> GetBooks(BookSearch bookSearch);

        public IEnumerable<BookUploadRecord> GetBookUploadRecords(string user);

        public bool UploadBook (BookUpload book);

    }
}

