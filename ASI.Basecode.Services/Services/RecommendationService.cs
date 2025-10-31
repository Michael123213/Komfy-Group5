using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Services.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBorrowingRepository _borrowingRepository;

        public RecommendationService(
            IBookRepository bookRepository,
            IBorrowingRepository borrowingRepository)
        {
            _bookRepository = bookRepository;
            _borrowingRepository = borrowingRepository;
        }

        // ADVANCED FEATURE #4: Get recommendations by preferences
        public List<BookModel> GetRecommendationsByPreferences(string genre = null, string author = null, int minRating = 0)
        {
            var books = _bookRepository.GetBooks().ToList();

            if (!string.IsNullOrEmpty(genre))
            {
                books = books.Where(b => b.Genre.ToLower().Contains(genre.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(author))
            {
                books = books.Where(b => b.Author.ToLower().Contains(author.ToLower())).ToList();
            }

            if (minRating > 0)
            {
                books = books.Where(b => b.Reviews.Any() && b.Reviews.Average(r => r.Rating) >= minRating).ToList();
            }

            // Sort by rating and borrow count
            books = books.OrderByDescending(b => b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0)
                        .ThenByDescending(b => b.BorrowCount)
                        .Take(10)
                        .ToList();

            return books.Select(b => MapToBookModel(b)).ToList();
        }

        // ADVANCED FEATURE #4: Get similar books
        public List<BookModel> GetSimilarBooks(int bookId, int count = 5)
        {
            var targetBook = _bookRepository.GetBookById(bookId);
            if (targetBook == null)
            {
                return new List<BookModel>();
            }

            var similarBooks = _bookRepository.GetBooks()
                .Where(b => b.BookID != bookId &&
                           (b.Genre == targetBook.Genre || b.Author == targetBook.Author))
                .ToList()
                .OrderByDescending(b =>
                    (b.Genre == targetBook.Genre ? 2 : 0) +
                    (b.Author == targetBook.Author ? 1 : 0) +
                    (b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) / 10 : 0))
                .Take(count)
                .ToList();

            return similarBooks.Select(b => MapToBookModel(b)).ToList();
        }

        // ADVANCED FEATURE #4: Get recommendations for user based on borrowing history
        public List<BookModel> GetRecommendationsForUser(string userId, int count = 10)
        {
            // Get user's borrowing history
            var userBorrowings = _borrowingRepository.GetBorrowingsByUserId(userId).ToList();

            if (!userBorrowings.Any())
            {
                // If no history, return trending books
                return GetTrendingBooks(count);
            }

            // Get genres and authors from user's history
            var borrowedGenres = userBorrowings.Select(b => b.Book.Genre).Distinct().ToList();
            var borrowedAuthors = userBorrowings.Select(b => b.Book.Author).Distinct().ToList();
            var borrowedBookIds = userBorrowings.Select(b => b.BookID).ToList();

            // Find books in similar genres or by same authors that user hasn't borrowed
            var recommendations = _bookRepository.GetBooks()
                .Where(b => !borrowedBookIds.Contains(b.BookID) &&
                           (borrowedGenres.Contains(b.Genre) || borrowedAuthors.Contains(b.Author)))
                .ToList()
                .OrderByDescending(b => b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0)
                .ThenByDescending(b => b.BorrowCount)
                .Take(count)
                .ToList();

            return recommendations.Select(b => MapToBookModel(b)).ToList();
        }

        // ADVANCED FEATURE #4: Get trending books
        public List<BookModel> GetTrendingBooks(int count = 10)
        {
            var books = _bookRepository.GetBooks()
                .ToList()
                .OrderByDescending(b => b.BorrowCount + b.ViewCount)
                .ThenByDescending(b => b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0)
                .Take(count)
                .ToList();

            return books.Select(b => MapToBookModel(b)).ToList();
        }

        // ADVANCED FEATURE #4: Process chatbot query
        public ChatbotResponseModel ProcessChatbotQuery(string query, string userId = null)
        {
            query = query.ToLower();
            var response = new ChatbotResponseModel();

            // Parse query for keywords
            if (query.Contains("genre") || query.Contains("fiction") || query.Contains("fantasy") ||
                query.Contains("mystery") || query.Contains("romance") || query.Contains("science"))
            {
                // Extract genre from query
                string genre = ExtractGenre(query);
                response.RecommendedBooks = GetRecommendationsByPreferences(genre: genre);
                response.Message = $"Here are some great {genre} books for you:";
                response.QueryType = "genre";
            }
            else if (query.Contains("author") || query.Contains("by") || query.Contains("written by"))
            {
                // Extract author from query
                string author = ExtractAuthor(query);
                response.RecommendedBooks = GetRecommendationsByPreferences(author: author);
                response.Message = $"Here are books by {author}:";
                response.QueryType = "author";
            }
            else if (query.Contains("trending") || query.Contains("popular") || query.Contains("hot"))
            {
                response.RecommendedBooks = GetTrendingBooks();
                response.Message = "Here are the trending books right now:";
                response.QueryType = "trending";
            }
            else if (query.Contains("rated") || query.Contains("best") || query.Contains("top"))
            {
                response.RecommendedBooks = GetRecommendationsByPreferences(minRating: 4);
                response.Message = "Here are our top-rated books:";
                response.QueryType = "top-rated";
            }
            else if (!string.IsNullOrEmpty(userId))
            {
                // Personalized recommendations
                response.RecommendedBooks = GetRecommendationsForUser(userId);
                response.Message = "Based on your reading history, you might like these:";
                response.QueryType = "personalized";
            }
            else
            {
                // Default: trending books
                response.RecommendedBooks = GetTrendingBooks();
                response.Message = "Here are some popular books you might enjoy:";
                response.QueryType = "default";
            }

            return response;
        }

        // Helper method to map Book entity to BookModel
        private BookModel MapToBookModel(Data.Models.Book book)
        {
            return new BookModel
            {
                BookID = book.BookID,
                Title = book.Title,
                BookCode = book.BookCode,
                Genre = book.Genre,
                Author = book.Author,
                Publisher = book.Publisher,
                Status = book.Status,
                DatePublished = book.DatePublished,
                Description = book.Description,
                CoverImagePath = book.CoverImagePath,
                IsEbook = book.IsEbook,
                EbookPath = book.EbookPath,
                ViewCount = book.ViewCount,
                BorrowCount = book.BorrowCount,
                AverageRating = book.Reviews.Any() ? book.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = book.Reviews.Count
            };
        }

        // Helper method to extract genre from query
        private string ExtractGenre(string query)
        {
            var genres = new[] { "fiction", "fantasy", "mystery", "romance", "science", "thriller", "horror", "biography", "history" };
            foreach (var genre in genres)
            {
                if (query.Contains(genre))
                {
                    return genre;
                }
            }
            return "fiction"; // default
        }

        // Helper method to extract author from query
        private string ExtractAuthor(string query)
        {
            // Simple extraction - in production, use NLP
            var words = query.Split(' ');
            for (int i = 0; i < words.Length - 1; i++)
            {
                if (words[i] == "by" || words[i] == "author")
                {
                    return words[i + 1];
                }
            }
            return "";
        }
    }
}