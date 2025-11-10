using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
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
        /// Returns Home View.
        /// </summary>
        /// <returns> Home View </returns>
        public IActionResult Index()
        {
            // Get all books with their review data
            var allBooks = _bookService.GetAvailableBooks();
            foreach (var book in allBooks)
            {
                book.AverageRating = _reviewService.GetAverageRatingForBook(book.BookID);
                var reviews = _reviewService.GetReviewsByBookId(book.BookID);
                book.ReviewCount = reviews.Count;
            }

            // Most Borrowed Books (Top 5)
            var mostBorrowedBooks = allBooks
                .OrderByDescending(b => b.BorrowCount)
                .Take(5)
                .ToList();

            // Top Rated Books (Top 5, only books with reviews)
            var topRatedBooks = allBooks
                .Where(b => b.ReviewCount > 0)
                .OrderByDescending(b => b.AverageRating)
                .ThenByDescending(b => b.ReviewCount)
                .Take(5)
                .ToList();

            // Top Borrowers (Top 5 users by number of RETURNED books)
            var allBorrowings = _borrowingService.GetAllBorrowings();

            // Debug: Check if we have any borrowings at all
            System.Diagnostics.Debug.WriteLine($"Total borrowings: {allBorrowings?.Count ?? 0}");

            if (allBorrowings != null && allBorrowings.Any())
            {
                // Count borrowings by status for debugging
                var statusCounts = allBorrowings.GroupBy(b => b.Status).Select(g => new { Status = g.Key, Count = g.Count() });
                foreach (var status in statusCounts)
                {
                    System.Diagnostics.Debug.WriteLine($"Status '{status.Status}': {status.Count} borrowings");
                }
            }

            // Filter: only count returned books with valid UserId
            var returnedBorrowings = allBorrowings?
                .Where(b => !string.IsNullOrEmpty(b.Status) &&
                           b.Status.Equals("Returned", System.StringComparison.OrdinalIgnoreCase) &&
                           !string.IsNullOrEmpty(b.UserId))
                .ToList() ?? new List<BorrowingModel>();

            System.Diagnostics.Debug.WriteLine($"Returned borrowings with valid UserId: {returnedBorrowings.Count}");

            // Debug: Print some returned borrowing details
            foreach (var b in returnedBorrowings.Take(3))
            {
                System.Diagnostics.Debug.WriteLine($"  Borrowing - UserId: '{b.UserId}', UserName: '{b.UserName}', UserEmail: '{b.UserEmail}', Status: '{b.Status}'");
            }

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

            System.Diagnostics.Debug.WriteLine($"Top borrowers count: {topBorrowers?.Count ?? 0}");
            foreach (var borrower in topBorrowers)
            {
                System.Diagnostics.Debug.WriteLine($"  Borrower - UserId: '{borrower.UserId}', Name: '{borrower.UserName}', Count: {borrower.BorrowCount}");
            }

            ViewBag.MostBorrowedBooks = mostBorrowedBooks;
            ViewBag.TopRatedBooks = topRatedBooks;
            ViewBag.TopBorrowers = topBorrowers;

            return View();
        }
    }
}
