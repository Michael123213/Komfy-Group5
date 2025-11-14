using ASI.Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    /// <summary>
    /// Controller for generating and displaying system reports and analytics.
    /// Restricted to Admin role users only.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly IBorrowingService _borrowingService;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly IReviewService _reviewService;

        /// <summary>
        /// Initializes a new instance of the ReportsController with required service dependencies.
        /// </summary>
        /// <param name="logger">Logger instance for error tracking and debugging</param>
        /// <param name="borrowingService">Service for borrowing-related data operations</param>
        /// <param name="bookService">Service for book-related data operations</param>
        /// <param name="userService">Service for user-related data operations</param>
        /// <param name="reviewService">Service for review-related data operations</param>
        public ReportsController(
            ILogger<ReportsController> logger,
            IBorrowingService borrowingService,
            IBookService bookService,
            IUserService userService,
            IReviewService reviewService)
        {
            _logger = logger;
            _borrowingService = borrowingService;
            _bookService = bookService;
            _userService = userService;
            _reviewService = reviewService;
        }

        /// <summary>
        /// Displays the reports dashboard with three main report sections:
        /// Borrowing Report, Inventory Report, and Member Report.
        /// </summary>
        /// <returns>Reports view with aggregated statistics and chart data</returns>
        public IActionResult Index()
        {
            try
            {
                // ============================================
                // BORROWING REPORT DATA
                // ============================================
                var allBorrowings = _borrowingService.GetAllBorrowings();
                
                // Calculate borrowing statistics by status
                var activeBorrowings = allBorrowings.Count(b => b.Status == "Active");
                var overdueBorrowings = allBorrowings.Count(b => b.Status == "Overdue");
                var returnedBorrowings = allBorrowings.Count(b => b.Status == "Returned");
                var cancelledBorrowings = allBorrowings.Count(b => b.Status == "Cancelled");

                // Calculate borrowing trends over the last 6 months for chart visualization
                var sixMonthsAgo = DateTime.Now.AddMonths(-6);
                var borrowingsByMonth = allBorrowings
                    .Where(b => b.BorrowDate >= sixMonthsAgo)
                    .GroupBy(b => new { b.BorrowDate.Year, b.BorrowDate.Month })
                    .Select(g => new
                    {
                        Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Month)
                    .ToList();

                // Pass borrowing statistics to view
                ViewBag.ActiveBorrowings = activeBorrowings;
                ViewBag.OverdueBorrowings = overdueBorrowings;
                ViewBag.ReturnedBorrowings = returnedBorrowings;
                ViewBag.CancelledBorrowings = cancelledBorrowings;
                ViewBag.TotalBorrowings = allBorrowings.Count;
                ViewBag.BorrowingsByMonth = borrowingsByMonth;

                // ============================================
                // INVENTORY (BOOKS) REPORT DATA
                // ============================================
                var allBooks = _bookService.GetAllBooks();
                
                // Calculate book inventory statistics by availability status
                var availableBooks = allBooks.Count(b => b.Status == "Available");
                var borrowedBooks = allBooks.Count(b => b.Status == "Borrowed");
                var totalBooks = allBooks.Count;

                // Group books by genre for distribution analysis (top 10 genres)
                var booksByGenre = allBooks
                    .Where(b => !string.IsNullOrWhiteSpace(b.Genre))
                    .GroupBy(b => b.Genre)
                    .Select(g => new
                    {
                        Genre = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToList();

                // Identify most popular books based on total borrow count (top 10)
                // Truncate long titles for better chart display
                var topBorrowedBooks = allBooks
                    .OrderByDescending(b => b.BorrowCount)
                    .Take(10)
                    .Select(b => new
                    {
                        Title = b.Title.Length > 30 ? b.Title.Substring(0, 30) + "..." : b.Title,
                        BorrowCount = b.BorrowCount
                    })
                    .ToList();

                // Pass inventory statistics to view
                ViewBag.AvailableBooks = availableBooks;
                ViewBag.BorrowedBooks = borrowedBooks;
                ViewBag.TotalBooks = totalBooks;
                ViewBag.BooksByGenre = booksByGenre;
                ViewBag.TopBorrowedBooks = topBorrowedBooks;

                // ============================================
                // MEMBER REPORT DATA
                // ============================================
                var allUsers = _userService.GetAllUsers();
                var totalMembers = allUsers.Count;

                // Group members by role for role distribution analysis
                var membersByRole = allUsers
                    .GroupBy(u => u.Role ?? "User")
                    .Select(g => new
                    {
                        Role = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                // Calculate top borrowers based on completed (returned) transactions
                // Groups by UserId and counts total returned books per user
                var topBorrowers = allBorrowings
                    .Where(b => b.Status == "Returned")
                    .GroupBy(b => b.UserId)
                    .Select(g => new
                    {
                        UserName = g.FirstOrDefault()?.UserName ?? "Unknown",
                        BorrowCount = g.Count()
                    })
                    .OrderByDescending(x => x.BorrowCount)
                    .Take(10)
                    .ToList();

                // Calculate active vs inactive member statistics
                // Active members are those who have at least one borrowing transaction
                var activeMemberIds = allBorrowings.Select(b => b.UserId).Distinct().ToList();
                var activeMembers = activeMemberIds.Count;
                var inactiveMembers = totalMembers - activeMembers;

                ViewBag.TotalMembers = totalMembers;
                ViewBag.MembersByRole = membersByRole;
                ViewBag.TopBorrowers = topBorrowers;
                ViewBag.ActiveMembers = activeMembers;
                ViewBag.InactiveMembers = inactiveMembers;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reports");
                TempData["ErrorMessage"] = "Error loading reports. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
