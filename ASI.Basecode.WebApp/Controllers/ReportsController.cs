using ASI.Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly IBorrowingService _borrowingService;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly IReviewService _reviewService;

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

        // GET: /Reports/Index
        public IActionResult Index()
        {
            try
            {
                // Borrowing Report Data
                var allBorrowings = _borrowingService.GetAllBorrowings();
                var activeBorrowings = allBorrowings.Count(b => b.Status == "Active");
                var overdueBorrowings = allBorrowings.Count(b => b.Status == "Overdue");
                var returnedBorrowings = allBorrowings.Count(b => b.Status == "Returned");
                var cancelledBorrowings = allBorrowings.Count(b => b.Status == "Cancelled");

                // Borrowing trends by month (last 6 months)
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

                ViewBag.ActiveBorrowings = activeBorrowings;
                ViewBag.OverdueBorrowings = overdueBorrowings;
                ViewBag.ReturnedBorrowings = returnedBorrowings;
                ViewBag.CancelledBorrowings = cancelledBorrowings;
                ViewBag.TotalBorrowings = allBorrowings.Count;
                ViewBag.BorrowingsByMonth = borrowingsByMonth;

                // Inventory (Books) Report Data
                var allBooks = _bookService.GetAllBooks();
                var availableBooks = allBooks.Count(b => b.Status == "Available");
                var borrowedBooks = allBooks.Count(b => b.Status == "Borrowed");
                var totalBooks = allBooks.Count;

                // Books by genre
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

                // Top borrowed books
                var topBorrowedBooks = allBooks
                    .OrderByDescending(b => b.BorrowCount)
                    .Take(10)
                    .Select(b => new
                    {
                        Title = b.Title.Length > 30 ? b.Title.Substring(0, 30) + "..." : b.Title,
                        BorrowCount = b.BorrowCount
                    })
                    .ToList();

                ViewBag.AvailableBooks = availableBooks;
                ViewBag.BorrowedBooks = borrowedBooks;
                ViewBag.TotalBooks = totalBooks;
                ViewBag.BooksByGenre = booksByGenre;
                ViewBag.TopBorrowedBooks = topBorrowedBooks;

                // Member Report Data
                var allUsers = _userService.GetAllUsers();
                var totalMembers = allUsers.Count;

                // Members by role
                var membersByRole = allUsers
                    .GroupBy(u => u.Role ?? "User")
                    .Select(g => new
                    {
                        Role = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                // Top borrowers (users with most returned books)
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

                // Active vs Inactive members (members who have borrowed vs haven't)
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
