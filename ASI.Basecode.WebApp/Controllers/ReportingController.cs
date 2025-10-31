using ASI.Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ASI.Basecode.WebApp.Controllers
{
    [Authorize(Roles = "Admin")] // Only admins can access reports
    public class ReportingController : Controller
    {
        private readonly ILogger<ReportingController> _logger;
        private readonly IReportingService _reportingService;

        public ReportingController(ILogger<ReportingController> logger, IReportingService reportingService)
        {
            _logger = logger;
            _reportingService = reportingService;
        }

        // ADVANCED FEATURE #2: GET: /Reporting/Index (Main reporting page)
        public IActionResult Index()
        {
            return View();
        }

        // ADVANCED FEATURE #2: GET: /Reporting/BorrowingReport?status=Active
        public IActionResult BorrowingReport(string status = null)
        {
            try
            {
                var report = _reportingService.GetBorrowingReport(status);
                ViewBag.StatusFilter = status;
                return View(report);
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error generating borrowing report.";
                _logger.LogError(ex, "Error generating borrowing report.");
                return RedirectToAction("Index");
            }
        }

        // ADVANCED FEATURE #2: GET: /Reporting/InventoryReport?genre=Fiction
        public IActionResult InventoryReport(string genre = null, string author = null, string publisher = null)
        {
            try
            {
                var report = _reportingService.GetInventoryReport(genre, author, publisher);
                ViewBag.GenreFilter = genre;
                ViewBag.AuthorFilter = author;
                ViewBag.PublisherFilter = publisher;
                return View(report);
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error generating inventory report.";
                _logger.LogError(ex, "Error generating inventory report.");
                return RedirectToAction("Index");
            }
        }

        // ADVANCED FEATURE #2: GET: /Reporting/MemberReport
        public IActionResult MemberReport()
        {
            try
            {
                var report = _reportingService.GetMemberReport();
                return View(report);
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error generating member report.";
                _logger.LogError(ex, "Error generating member report.");
                return RedirectToAction("Index");
            }
        }
    }
}