using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Utilities
{
    /// <summary>
    /// Utility class to fetch books from Google Books API for database seeding
    /// </summary>
    public static class GoogleBooksSeeder
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string GOOGLE_BOOKS_API_BASE = "https://www.googleapis.com/books/v1/volumes";

        /// <summary>
        /// Fetches 50 books from Google Books API across various genres
        /// </summary>
        public static List<Book> GetSeedBooks()
        {
            var books = new List<Book>();
            var queries = new[]
            {
                "fiction", "science", "history", "biography", "technology",
                "mystery", "romance", "fantasy", "self-help", "business"
            };

            int booksPerQuery = 5; // 10 queries * 5 books = 50 books
            int bookIdCounter = 1;

            foreach (var query in queries)
            {
                try
                {
                    var task = FetchBooksFromAPIAsync(query, booksPerQuery);
                    task.Wait();
                    var apiBooks = task.Result;

                    foreach (var apiBook in apiBooks)
                    {
                        books.Add(ConvertToBook(apiBook, bookIdCounter++, query));
                    }
                }
                catch (Exception ex)
                {
                    // If API fails, use fallback data
                    Console.WriteLine($"Failed to fetch books for '{query}': {ex.Message}. Using fallback data.");
                    books.AddRange(GetFallbackBooks(query, booksPerQuery, ref bookIdCounter));
                }
            }

            return books.Take(50).ToList();
        }

        private static async Task<List<JsonElement>> FetchBooksFromAPIAsync(string query, int maxResults)
        {
            try
            {
                var url = $"{GOOGLE_BOOKS_API_BASE}?q={query}&maxResults={maxResults}&orderBy=relevance";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var document = JsonDocument.Parse(content);

                if (document.RootElement.TryGetProperty("items", out var items))
                {
                    // Clone the JsonElements to avoid disposal issues
                    var result = items.EnumerateArray().Select(item => item.Clone()).ToList();
                    document.Dispose();
                    return result;
                }

                document.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API call failed: {ex.Message}");
            }

            return new List<JsonElement>();
        }

        private static Book ConvertToBook(JsonElement item, int bookId, string genre)
        {
            var volumeInfo = item.GetProperty("volumeInfo");

            // Extract title
            var title = volumeInfo.TryGetProperty("title", out var titleProp)
                ? titleProp.GetString()
                : "Unknown Title";

            // Extract authors
            var authors = "Unknown Author";
            if (volumeInfo.TryGetProperty("authors", out var authorsProp))
            {
                authors = string.Join(", ", authorsProp.EnumerateArray().Select(a => a.GetString()));
            }

            // Extract publisher
            var publisher = volumeInfo.TryGetProperty("publisher", out var publisherProp)
                ? publisherProp.GetString()
                : "Unknown Publisher";

            // Extract published date
            DateTime? publishedDate = null;
            if (volumeInfo.TryGetProperty("publishedDate", out var dateProp))
            {
                var dateStr = dateProp.GetString();
                if (DateTime.TryParse(dateStr, out var parsedDate))
                {
                    publishedDate = parsedDate;
                }
            }

            // Extract description
            var description = volumeInfo.TryGetProperty("description", out var descProp)
                ? TruncateString(descProp.GetString(), 2000)
                : "No description available.";

            // Extract cover image
            var coverImage = "";
            if (volumeInfo.TryGetProperty("imageLinks", out var imageLinks))
            {
                if (imageLinks.TryGetProperty("thumbnail", out var thumbnail))
                {
                    coverImage = thumbnail.GetString();
                }
                else if (imageLinks.TryGetProperty("smallThumbnail", out var smallThumbnail))
                {
                    coverImage = smallThumbnail.GetString();
                }
            }

            // Generate book code
            var bookCode = $"BK{bookId:D4}";

            return new Book
            {
                BookID = bookId,
                Title = TruncateString(title, 255),
                BookCode = bookCode,
                Author = TruncateString(authors, 150),
                Publisher = TruncateString(publisher, 150),
                Genre = CapitalizeFirst(genre),
                Status = "Available",
                DatePublished = publishedDate,
                Description = description,
                CoverImagePath = TruncateString(coverImage, 500),
                IsEbook = false,
                ViewCount = 0,
                BorrowCount = 0
            };
        }

        private static List<Book> GetFallbackBooks(string genre, int count, ref int bookIdCounter)
        {
            var books = new List<Book>();
            var fallbackData = new Dictionary<string, List<(string title, string author, string publisher, string coverUrl)>>
            {
                ["fiction"] = new List<(string, string, string, string)>
                {
                    ("To Kill a Mockingbird", "Harper Lee", "J.B. Lippincott & Co.", "https://books.google.com/books/content?id=PGR2AwAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("1984", "George Orwell", "Secker & Warburg", "https://books.google.com/books/content?id=kotPYEqx7kMC&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("Pride and Prejudice", "Jane Austen", "T. Egerton", "https://books.google.com/books/content?id=s1gVAAAAYAAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("The Great Gatsby", "F. Scott Fitzgerald", "Charles Scribner's Sons", "https://books.google.com/books/content?id=iXn5U2IzVH0C&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("The Catcher in the Rye", "J.D. Salinger", "Little, Brown and Company", "https://books.google.com/books/content?id=PCDengEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api")
                },
                ["science"] = new List<(string, string, string, string)>
                {
                    ("A Brief History of Time", "Stephen Hawking", "Bantam Dell", "https://books.google.com/books/content?id=1Q4aAAAAMAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api"),
                    ("The Origin of Species", "Charles Darwin", "John Murray", "https://books.google.com/books/content?id=8rBbAAAAQAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api"),
                    ("Cosmos", "Carl Sagan", "Random House", "https://books.google.com/books/content?id=xFSyDwAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("The Selfish Gene", "Richard Dawkins", "Oxford University Press", "https://books.google.com/books/content?id=WkHO9HI7koEC&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("Sapiens", "Yuval Noah Harari", "Harper", "https://books.google.com/books/content?id=1EiJAwAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api")
                },
                ["history"] = new List<(string, string, string, string)>
                {
                    ("Guns, Germs, and Steel", "Jared Diamond", "W. W. Norton", "https://books.google.com/books/content?id=PWnWRFRGNr8C&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("The Diary of a Young Girl", "Anne Frank", "Contact Publishing", "https://books.google.com/books/content?id=K1xaCwAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("A People's History of the United States", "Howard Zinn", "Harper & Row", "https://books.google.com/books/content?id=hzmXFi5NgwYC&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("SPQR: A History of Ancient Rome", "Mary Beard", "Liveright", "https://books.google.com/books/content?id=6w4JDAAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("The History of the Decline and Fall of the Roman Empire", "Edward Gibbon", "Strahan & Cadell", "https://books.google.com/books/content?id=GZFHAQAAMAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api")
                },
                ["biography"] = new List<(string, string, string, string)>
                {
                    ("Steve Jobs", "Walter Isaacson", "Simon & Schuster", "https://books.google.com/books/content?id=8U2oAAAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("The Autobiography of Malcolm X", "Malcolm X", "Grove Press", "https://books.google.com/books/content?id=jPiCDwAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("Long Walk to Freedom", "Nelson Mandela", "Little, Brown and Company", "https://books.google.com/books/content?id=W5fQAAAAMAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api"),
                    ("Einstein: His Life and Universe", "Walter Isaacson", "Simon & Schuster", "https://books.google.com/books/content?id=cdz6hLFx72kC&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("Becoming", "Michelle Obama", "Crown Publishing", "https://books.google.com/books/content?id=hi09DwAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api")
                },
                ["technology"] = new List<(string, string, string, string)>
                {
                    ("The Innovators", "Walter Isaacson", "Simon & Schuster", "https://books.google.com/books/content?id=MQiJAwAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("Clean Code", "Robert C. Martin", "Prentice Hall", "https://books.google.com/books/content?id=hjEFCAAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("The Pragmatic Programmer", "Andrew Hunt", "Addison-Wesley", "https://books.google.com/books/content?id=5wBQEp6ruIAC&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("Artificial Intelligence: A Modern Approach", "Stuart Russell", "Prentice Hall", "https://books.google.com/books/content?id=8jZBksh-bUMC&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("Code Complete", "Steve McConnell", "Microsoft Press", "https://books.google.com/books/content?id=LpVCAwAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api")
                },
                ["mystery"] = new List<(string, string, string, string)>
                {
                    ("The Adventures of Sherlock Holmes", "Arthur Conan Doyle", "George Newnes", "https://books.google.com/books/content?id=AwULAAAAYAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api"),
                    ("Gone Girl", "Gillian Flynn", "Crown Publishing", "https://books.google.com/books/content?id=ol3xQBhnX8sC&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("The Girl with the Dragon Tattoo", "Stieg Larsson", "Norstedts Förlag", "https://books.google.com/books/content?id=bTfTqV_bBc4C&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("And Then There Were None", "Agatha Christie", "Collins Crime Club", "https://books.google.com/books/content?id=wJVfDwAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("The Da Vinci Code", "Dan Brown", "Doubleday", "https://books.google.com/books/content?id=KQZCPgAACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api")
                },
                ["romance"] = new List<(string, string, string, string)>
                {
                    ("Pride and Prejudice", "Jane Austen", "T. Egerton", "https://books.google.com/books/content?id=s1gVAAAAYAAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("The Notebook", "Nicholas Sparks", "Warner Books", "https://books.google.com/books/content?id=11YtDgAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("Outlander", "Diana Gabaldon", "Delacorte Press", "https://books.google.com/books/content?id=X5bDCwAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("Me Before You", "Jojo Moyes", "Penguin Books", "https://books.google.com/books/content?id=FJIjAwAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("The Fault in Our Stars", "John Green", "Dutton Books", "https://books.google.com/books/content?id=Tp8bpHaEHy8C&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api")
                },
                ["fantasy"] = new List<(string, string, string, string)>
                {
                    ("The Hobbit", "J.R.R. Tolkien", "George Allen & Unwin", "https://books.google.com/books/content?id=pD6arNyKyi8C&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("Harry Potter and the Sorcerer's Stone", "J.K. Rowling", "Bloomsbury", "https://books.google.com/books/content?id=wrOQLV6xB-wC&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("A Game of Thrones", "George R.R. Martin", "Bantam Books", "https://books.google.com/books/content?id=5NomkK4EV68C&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("The Name of the Wind", "Patrick Rothfuss", "DAW Books", "https://books.google.com/books/content?id=jZAdzKDYUhQC&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("The Way of Kings", "Brandon Sanderson", "Tor Books", "https://books.google.com/books/content?id=X8CWBQAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api")
                },
                ["self-help"] = new List<(string, string, string, string)>
                {
                    ("The 7 Habits of Highly Effective People", "Stephen Covey", "Free Press", "https://books.google.com/books/content?id=qL0ELSLmT3EC&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("How to Win Friends and Influence People", "Dale Carnegie", "Simon & Schuster", "https://books.google.com/books/content?id=1rW-QpIAs8UC&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("Atomic Habits", "James Clear", "Avery", "https://books.google.com/books/content?id=XfFvDwAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("Think and Grow Rich", "Napoleon Hill", "The Ralston Society", "https://books.google.com/books/content?id=oZWbugEACAAJ&printsec=frontcover&img=1&zoom=1&source=gbs_api"),
                    ("The Power of Now", "Eckhart Tolle", "New World Library", "https://books.google.com/books/content?id=52oSDgAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api")
                },
                ["business"] = new List<(string, string, string, string)>
                {
                    ("The Lean Startup", "Eric Ries", "Crown Business", "https://books.google.com/books/content?id=tvfyz-4JILwC&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("Good to Great", "Jim Collins", "HarperBusiness", "https://books.google.com/books/content?id=ay8lc5nTimEC&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("Zero to One", "Peter Thiel", "Crown Business", "https://books.google.com/books/content?id=nneBAwAAQBAJ&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("The Innovator's Dilemma", "Clayton M. Christensen", "Harvard Business Review Press", "https://books.google.com/books/content?id=SIexi_qgq2gC&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api"),
                    ("Built to Last", "Jim Collins", "HarperBusiness", "https://books.google.com/books/content?id=GqT-lxk24ssC&printsec=frontcover&img=1&zoom=1&edge=curl&source=gbs_api")
                }
            };

            if (fallbackData.ContainsKey(genre.ToLower()))
            {
                var genreBooks = fallbackData[genre.ToLower()].Take(count);
                foreach (var book in genreBooks)
                {
                    books.Add(new Book
                    {
                        BookID = bookIdCounter,
                        Title = book.title,
                        BookCode = $"BK{bookIdCounter:D4}",
                        Author = book.author,
                        Publisher = book.publisher,
                        Genre = CapitalizeFirst(genre),
                        Status = "Available",
                        Description = $"A classic {genre} book.",
                        CoverImagePath = book.coverUrl,
                        IsEbook = false,
                        ViewCount = 0,
                        BorrowCount = 0
                    });
                    bookIdCounter++;
                }
            }

            return books;
        }

        private static string TruncateString(string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return str.Length <= maxLength ? str : str.Substring(0, maxLength - 3) + "...";
        }

        private static string CapitalizeFirst(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }
    }
}
