using ASI.Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ASI.Basecode.WebApp.Controllers
{
    [Authorize(Roles = "Admin")] // Only admins can access analytics
    public class AnalyticsController : Controller
    {
        private readonly ILogger<AnalyticsController> _logger;
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(ILogger<AnalyticsController> logger, IAnalyticsService analyticsService)
        {
            _logger = logger;
            _analyticsService = analyticsService;
        }

        // ADVANCED FEATURE #1: GET: /Analytics/Dashboard (Main analytics dashboard)
        public IActionResult Dashboard()
        {
            try
            {
                var summary = _analyticsService.GetDashboardSummary();
                return View(summary);
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading dashboard.";
                _logger.LogError(ex, "Error loading analytics dashboard.");
                return RedirectToAction("Index", "Home");
            }
        }

        // ADVANCED FEATURE #1: GET: /Analytics/MostBorrowed
        public IActionResult MostBorrowed(int count = 10)
        {
            try
            {
                var books = _analyticsService.GetMostBorrowedBooks(count);
                ViewBag.Title = "Most Borrowed Books";
                ViewBag.Count = count;
                return View("BookAnalytics", books);
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading most borrowed books.";
                _logger.LogError(ex, "Error loading most borrowed books.");
                return RedirectToAction("Dashboard");
            }
        }

        // ADVANCED FEATURE #1: GET: /Analytics/MostViewed
        public IActionResult MostViewed(int count = 10)
        {
            try
            {
                var books = _analyticsService.GetMostViewedBooks(count);
                ViewBag.Title = "Most Viewed Books";
                ViewBag.Count = count;
                return View("BookAnalytics", books);
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading most viewed books.";
                _logger.LogError(ex, "Error loading most viewed books.");
                return RedirectToAction("Dashboard");
            }
        }

        // ADVANCED FEATURE #1: GET: /Analytics/TopRated
        public IActionResult TopRated(int count = 10)
        {
            try
            {
                var books = _analyticsService.GetTopRatedBooks(count);
                ViewBag.Title = "Top Rated Books";
                ViewBag.Count = count;
                return View("BookAnalytics", books);
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading top rated books.";
                _logger.LogError(ex, "Error loading top rated books.");
                return RedirectToAction("Dashboard");
            }
        }

        // ADVANCED FEATURE #1: GET: /Analytics/TopBorrowers
        public IActionResult TopBorrowers(int count = 10)
        {
            try
            {
                var borrowers = _analyticsService.GetTopBorrowers(count);
                ViewBag.Count = count;
                return View(borrowers);
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading top borrowers.";
                _logger.LogError(ex, "Error loading top borrowers.");
                return RedirectToAction("Dashboard");
            }
        }
    }
}