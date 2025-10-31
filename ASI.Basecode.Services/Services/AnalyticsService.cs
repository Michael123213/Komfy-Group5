using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Services.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBorrowingRepository _borrowingRepository;
        private readonly IReviewRepository _reviewRepository;

        public AnalyticsService(
            IBookRepository bookRepository,
            IUserRepository userRepository,
            IBorrowingRepository borrowingRepository,
            IReviewRepository reviewRepository)
        {
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _borrowingRepository = borrowingRepository;
            _reviewRepository = reviewRepository;
        }

        // ADVANCED FEATURE #1: Most Borrowed Books
        public List<BookModel> GetMostBorrowedBooks(int count = 10)
        {
            var books = _bookRepository.GetBooks()
                .OrderByDescending(b => b.BorrowCount)
                .Take(count)
                .ToList();

            return books.Select(b => new BookModel
            {
                BookID = b.BookID,
                Title = b.Title,
                BookCode = b.BookCode,
                Genre = b.Genre,
                Author = b.Author,
                Publisher = b.Publisher,
                Status = b.Status,
                DatePublished = b.DatePublished,
                Description = b.Description,
                CoverImagePath = b.CoverImagePath,
                IsEbook = b.IsEbook,
                EbookPath = b.EbookPath,
                ViewCount = b.ViewCount,
                BorrowCount = b.BorrowCount,
                AverageRating = b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = b.Reviews.Count
            }).ToList();
        }

        // ADVANCED FEATURE #1: Most Viewed Books
        public List<BookModel> GetMostViewedBooks(int count = 10)
        {
            var books = _bookRepository.GetBooks()
                .OrderByDescending(b => b.ViewCount)
                .Take(count)
                .ToList();

            return books.Select(b => new BookModel
            {
                BookID = b.BookID,
                Title = b.Title,
                BookCode = b.BookCode,
                Genre = b.Genre,
                Author = b.Author,
                Publisher = b.Publisher,
                Status = b.Status,
                DatePublished = b.DatePublished,
                Description = b.Description,
                CoverImagePath = b.CoverImagePath,
                IsEbook = b.IsEbook,
                EbookPath = b.EbookPath,
                ViewCount = b.ViewCount,
                BorrowCount = b.BorrowCount,
                AverageRating = b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = b.Reviews.Count
            }).ToList();
        }

        // ADVANCED FEATURE #1: Top Borrowers
        public List<UserAnalyticsModel> GetTopBorrowers(int count = 10)
        {
            var userBorrowingStats = _borrowingRepository.GetBorrowings()
                .GroupBy(b => b.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    User = g.First().User,
                    TotalBorrowings = g.Count(),
                    ActiveBorrowings = g.Count(b => b.Status == "Active"),
                    OverdueBorrowings = g.Count(b => b.Status == "Overdue")
                })
                .OrderByDescending(x => x.TotalBorrowings)
                .Take(count)
                .ToList();

            return userBorrowingStats.Select(u => new UserAnalyticsModel
            {
                UserId = u.UserId,
                Name = u.User?.Name,
                Email = u.User?.Email,
                TotalBorrowings = u.TotalBorrowings,
                ActiveBorrowings = u.ActiveBorrowings,
                OverdueBorrowings = u.OverdueBorrowings
            }).ToList();
        }

        // ADVANCED FEATURE #1: Top Rated Books
        public List<BookModel> GetTopRatedBooks(int count = 10)
        {
            var books = _bookRepository.GetBooks()
                .Where(b => b.Reviews.Any()) // Only books with reviews
                .ToList() // Execute query first
                .Select(b => new
                {
                    Book = b,
                    AverageRating = b.Reviews.Average(r => r.Rating)
                })
                .OrderByDescending(x => x.AverageRating)
                .Take(count)
                .ToList();

            return books.Select(x => new BookModel
            {
                BookID = x.Book.BookID,
                Title = x.Book.Title,
                BookCode = x.Book.BookCode,
                Genre = x.Book.Genre,
                Author = x.Book.Author,
                Publisher = x.Book.Publisher,
                Status = x.Book.Status,
                DatePublished = x.Book.DatePublished,
                Description = x.Book.Description,
                CoverImagePath = x.Book.CoverImagePath,
                IsEbook = x.Book.IsEbook,
                EbookPath = x.Book.EbookPath,
                ViewCount = x.Book.ViewCount,
                BorrowCount = x.Book.BorrowCount,
                AverageRating = x.AverageRating,
                ReviewCount = x.Book.Reviews.Count
            }).ToList();
        }

        // ADVANCED FEATURE #1: Dashboard Summary Statistics
        public DashboardSummaryModel GetDashboardSummary()
        {
            var books = _bookRepository.GetBooks().ToList();
            var users = _userRepository.GetUsers().ToList();
            var borrowings = _borrowingRepository.GetBorrowings().ToList();
            var reviews = _reviewRepository.GetReviews().ToList();

            return new DashboardSummaryModel
            {
                // Book Statistics
                TotalBooks = books.Count,
                AvailableBooks = books.Count(b => b.Status == "Available"),
                BorrowedBooks = books.Count(b => b.Status == "Borrowed"),
                EbooksCount = books.Count(b => b.IsEbook),

                // User Statistics
                TotalMembers = users.Count(u => u.Role == "Member"),
                TotalAdmins = users.Count(u => u.Role == "Admin"),

                // Borrowing Statistics
                ActiveBorrowings = borrowings.Count(b => b.Status == "Active"),
                OverdueBorrowings = borrowings.Count(b => b.Status == "Overdue"),
                TotalBorrowingsAllTime = borrowings.Count,

                // Review Statistics
                TotalReviews = reviews.Count,
                AverageRatingAcrossAllBooks = reviews.Any() ? reviews.Average(r => r.Rating) : 0
            };
        }
    }
}