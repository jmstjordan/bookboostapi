// namespace BookBoostApi.Services;

// using System.Threading.Tasks;
// using BookBoostApi.Interfaces;
// using BookBoostApi.Models;
// using Microsoft.Extensions.Caching.Memory;

// public class BookService : IBookService
// {
//     private ProductService _productService;
//     private IMemoryCache _memoryCache;

//     public BookService(ProductService productService, IMemoryCache memoryCache)
//     {
//         _productService = productService;
//         _memoryCache = memoryCache;
//     }

//     private static readonly string[] Titles = new[]
//     {
//         "Sapiens: A Brief History of Humankind", "Twilight", "Born a Crime: Stories from a South African Childhood (One World Essentials)"
//     };

//     private static readonly string[] Authors = new[]
//     {
//         "Yuval Noah Harari", "Stephenie Meyer", "Trevor Noah"
//     };

//     private static readonly double[] Ratings = new[]
//     {
//         5.0, 4.8, 3.1
//     };

//     private static readonly int[] NumReviews = new[]
//     {
//         412, 1200, 99
//     };

//     private static readonly string[] SourceImageUrls = new[]
//     {
//         "https://m.media-amazon.com/images/I/716E6dQ4BXL._SY522_.jpg",
//         "https://m.media-amazon.com/images/I/71fFOMJM41L._SX522_.jpg",
//         "https://m.media-amazon.com/images/I/91mePFAgywL._SY522_.jpg",
//     };

//     private static readonly string[] Summaries = new[]
//     {
//         "From renowned historian Yuval Noah Harari comes a groundbreaking narrative of humanity’s creation and evolution—a #1 international bestseller—that explores the ways in which biology and history have defined us and enhanced our understanding of what it means to be human.",
//         "Isabella Swan's move to Forks, a small, perpetually rainy town in Washington, could have been the most boring move she ever made. But once she meets the mysterious and alluring Edward Cullen, Isabella's life takes a thrilling and terrifying turn. Up until now, Edward has managed to keep his vampire identity a secret in the small community he lives in, but now nobody is safe, especially Isabella, the person Edward holds most dear. The lovers find themselves balanced precariously on the point of a knife, between desire and danger.",
//         "Winner of the Thurber Prize for American Humor and an NAACP Image Award • Named one of the best books of the year by The New York Time, USA Today, San Francisco Chronicle, NPR, Esquire, Newsday, and Booklist",
//     };

//     private static readonly string[] Descriptions = new[]
//     {
//         """
//         One hundred thousand years ago, at least six different species of humans inhabited Earth. Yet today there is only one—homo sapiens. What happened to the others? And what may happen to us?

//         Most books about the history of humanity pursue either a historical or a biological approach, but Dr. Yuval Noah Harari breaks the mold with this highly original book that begins about 70,000 years ago with the appearance of modern cognition. From examining the role evolving humans have played in the global ecosystem to charting the rise of empires, Sapiens integrates history and science to reconsider accepted narratives, connect past developments with contemporary concerns, and examine specific events within the context of larger ideas.

//         Dr. Harari also compels us to look ahead, because over the last few decades humans have begun to bend laws of natural selection that have governed life for the past four billion years. We are acquiring the ability to design not only the world around us, but also ourselves. Where is this leading us, and what do we want to become?

//         Featuring 27 photographs, 6 maps, and 25 illustrations/diagrams, this provocative and insightful work is sure to spark debate and is essential reading for aficionados of Jared Diamond, James Gleick, Matt Ridley, Robert Wright, and Sharon Moalem.
//         """,
//         """
//         About three things I was certain.
//         First, Edward was a vampire.

//         Second, there was a part of him, and I didn't know how dominant that part might be, that thirsted for my blood.

//         And Third, I was unconditionally and irrevocably in love with him.

//         Isabella Swan's move to Forks, a small, perpetually rainy town in Washington, could have been the most boring move she ever made. But once she meets the mysterious and alluring Edward Cullen, Isabella's life takes a thrilling and terrifying turn. Up until now, Edward has managed to keep his vampire identity a secret in the small community he lives in, but now nobody is safe, especially Isabella, the person Edward holds most dear. The lovers find themselves balanced precariously on the point of a knife, between desire and danger.

//         Deeply sensuous and extraordinarily suspenseful, Twilight captures the struggle between defying our instincts and satisfying our desires. This is a love story with bite.
//         """,
//         """
//         Trevor Noah’s unlikely path from apartheid South Africa to the desk of The Daily Show began with a criminal act: his birth. Trevor was born to a white Swiss father and a black Xhosa mother at a time when such a union was punishable by five years in prison. Living proof of his parents’ indiscretion, Trevor was kept mostly indoors for the earliest years of his life, bound by the extreme and often absurd measures his mother took to hide him from a government that could, at any moment, steal him away. Finally liberated by the end of South Africa’s tyrannical white rule, Trevor and his mother set forth on a grand adventure, living openly and freely and embracing the opportunities won by a centuries-long struggle.

//         Born a Crime is the story of a mischievous young boy who grows into a restless young man as he struggles to find himself in a world where he was never supposed to exist. It is also the story of that young man’s relationship with his fearless, rebellious, and fervently religious mother—his teammate, a woman determined to save her son from the cycle of poverty, violence, and abuse that would ultimately threaten her own life.

//         The stories collected here are by turns hilarious, dramatic, and deeply affecting. Whether subsisting on caterpillars for dinner during hard times, being thrown from a moving car during an attempted kidnapping, or just trying to survive the life-and-death pitfalls of dating in high school, Trevor illuminates his curious world with an incisive wit and unflinching honesty. His stories weave together to form a moving and searingly funny portrait of a boy making his way through a damaged world in a dangerous time, armed only with a keen sense of humor and a mother’s unconventional, unconditional love.
//         """,
//     };

//     private static readonly int[] PageLength = new[]
//     {
//         464, 301, 202
//     };

//     private static readonly List<List<Category>> Categories = new List<List<Category>>
//     {
//         new List<Category>{Category.Bestseller, Category.Fiction},
//         new List<Category>{Category.NonFiction, Category.Romance},
//         new List<Category>{Category.NonFiction},
//     };

//     public Task<IEnumerable<Book>> SearchBooks(BookSearch bookSearch)
//     {
//         if (!_memoryCache.TryGetValue(CacheKeys.Entry, out DateTime cacheValue))
//         {
//             cacheValue = CurrentDateTime;

//             var cacheEntryOptions = new MemoryCacheEntryOptions()
//                 .SetSlidingExpiration(TimeSpan.FromSeconds(3));

//             _memoryCache.Set(CacheKeys.Entry, cacheValue, cacheEntryOptions);
//         }
//     }

//     public IEnumerable<BookUploadRecord> GetBookUploadRecords(string user)
//     {
//         return new List<BookUploadRecord>
//         {
//             new BookUploadRecord
//             {
//                 UploadDate = DateOnly.FromDateTime(DateTime.Now),
//                 User = "jmjordan",
//                 BookLinks = new List<BookLink>
//                 {
//                     new BookLink
//                     {
//                         ProductId = "B00ICN066A",
//                         ProductSource = ProductSource.Amazon,
//                         Url = "https://www.amazon.com/Sapiens-Humankind-Yuval-Noah-Harari-ebook/dp/B00ICN066A"
//                     }
//                 }
//             }
//         };
//     }

//     public async Task UploadBook(IEnumerable<BookUpload> books)
//     {
//         foreach(var book in books)
//         {
//             var productDetails = await _productService.GetProduct(book.Id, book.ProductSource);
//             // save off bookupload record to db
//         }
//     }
// }