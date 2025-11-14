using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    public class HomeController : ControllerBase<HomeController>
    {
        private readonly IBookService _bookService;
        private readonly IBorrowingService _borrowingService;
        private readonly IReviewService _reviewService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="mapper"></param>
        /// <param name="bookService"></param>
        /// <param name="borrowingService"></param>
        /// <param name="reviewService"></param>
        public HomeController(IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IBookService bookService,
                              IBorrowingService borrowingService,
                              IReviewService reviewService,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _bookService = bookService;
            _borrowingService = borrowingService;
            _reviewService = reviewService;
        }

        /// <summary>
        /// Displays the dashboard home page with book recommendations and analytics.
        /// Shows most borrowed books, top rated books, and top borrowers.
        /// </summary>
        /// <returns>Dashboard view with book statistics and recommendations</returns>
        public IActionResult Index()
        {
            // Retrieve all available books from the system
            var allBooks = _bookService.GetAvailableBooks();
            
            // Performance optimization: Load all reviews in a single query to avoid N+1 problem
            // Groups reviews by BookID for efficient lookup during book processing
            var bookIds = allBooks.Select(b => b.BookID).ToList();
            var allReviews = _reviewService.GetAllReviews()
                .Where(r => bookIds.Contains(r.BookID))
                .GroupBy(r => r.BookID)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            // Calculate review statistics for each book using pre-loaded review data
            foreach (var book in allBooks)
            {
                if (allReviews.TryGetValue(book.BookID, out var reviews))
                {
                    book.ReviewCount = reviews.Count;
                    book.AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
                }
                else
                {
                    book.ReviewCount = 0;
                    book.AverageRating = 0;
                }
            }

            // Calculate most borrowed books based on borrow count
            var mostBorrowedBooks = allBooks
                .OrderByDescending(b => b.BorrowCount)
                .Take(5)
                .ToList();

            // Calculate top rated books (only includes books that have received reviews)
            // Secondary sort by review count ensures books with more reviews rank higher when ratings are equal
            var topRatedBooks = allBooks
                .Where(b => b.ReviewCount > 0)
                .OrderByDescending(b => b.AverageRating)
                .ThenByDescending(b => b.ReviewCount)
                .Take(5)
                .ToList();

            // Calculate top borrowers based on completed (returned) book transactions
            var allBorrowings = _borrowingService.GetAllBorrowings();

            // Filter to only include returned borrowings with valid user identifiers
            // This ensures we only count completed transactions, not active or pending ones
            var returnedBorrowings = allBorrowings?
                .Where(b => !string.IsNullOrEmpty(b.Status) &&
                           b.Status.Equals("Returned", System.StringComparison.OrdinalIgnoreCase) &&
                           !string.IsNullOrEmpty(b.UserId))
                .ToList() ?? new List<BorrowingModel>();

            // Group borrowings by user and calculate total returned books per user
            var topBorrowers = returnedBorrowings
                .GroupBy(b => b.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    UserName = g.FirstOrDefault()?.UserName ?? "Unknown User",
                    UserEmail = g.FirstOrDefault()?.UserEmail ?? "No email",
                    BorrowCount = g.Count()
                })
                .OrderByDescending(u => u.BorrowCount)
                .Take(5)
                .ToList();

            ViewBag.MostBorrowedBooks = mostBorrowedBooks;
            ViewBag.TopRatedBooks = topRatedBooks;
            ViewBag.TopBorrowers = topBorrowers;

            return View();
        }

        /// <summary>
        /// Displays the public landing page for unauthenticated users.
        /// Shows featured books and a preview of available books in the library.
        /// </summary>
        /// <returns>Landing page view with featured books</returns>
        [AllowAnonymous]
        public IActionResult Landing()
        {
            // Retrieve all available books to display on the landing page
            var allBooks = _bookService.GetAvailableBooks();
            
            // Performance optimization: Load all reviews in a single query to avoid N+1 problem
            // Groups reviews by BookID for efficient lookup during book processing
            var bookIds = allBooks.Select(b => b.BookID).ToList();
            var allReviews = _reviewService.GetAllReviews()
                .Where(r => bookIds.Contains(r.BookID))
                .GroupBy(r => r.BookID)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            // Calculate review statistics for each book using pre-loaded review data
            foreach (var book in allBooks)
            {
                if (allReviews.TryGetValue(book.BookID, out var reviews))
                {
                    book.ReviewCount = reviews.Count;
                    book.AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
                }
                else
                {
                    book.ReviewCount = 0;
                    book.AverageRating = 0;
                }
            }

            // Select featured books based on highest ratings (books with reviews only)
            var featuredBooks = allBooks
                .Where(b => b.ReviewCount > 0)
                .OrderByDescending(b => b.AverageRating)
                .Take(4)
                .ToList();

            ViewBag.FeaturedBooks = featuredBooks;
            ViewBag.AllBooks = allBooks.Take(6).ToList();
            
            return View();
        }
    }
}
