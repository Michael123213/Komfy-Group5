using ASI.Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ASI.Basecode.WebApp.Controllers
{
    [AllowAnonymous] // Temporarily allow access to all users for development
    public class ChatbotController : Controller
    {
        private readonly ILogger<ChatbotController> _logger;
        private readonly IRecommendationService _recommendationService;

        public ChatbotController(ILogger<ChatbotController> logger, IRecommendationService recommendationService)
        {
            _logger = logger;
            _recommendationService = recommendationService;
        }

        // ADVANCED FEATURE #4: GET: /Chatbot/Index (Chatbot interface)
        public IActionResult Index()
        {
            return View();
        }

        // ADVANCED FEATURE #4: POST: /Chatbot/Ask (Process chatbot query)
        [HttpPost]
        public IActionResult Ask(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return Json(new { success = false, message = "Please enter a question." });
                }

                // Get current user ID if logged in
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var response = _recommendationService.ProcessChatbotQuery(query, userId);

                return Json(new
                {
                    success = true,
                    message = response.Message,
                    queryType = response.QueryType,
                    books = response.RecommendedBooks
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error processing chatbot query.");
                return Json(new { success = false, message = "Sorry, I encountered an error processing your request." });
            }
        }

        // ADVANCED FEATURE #4: GET: /Chatbot/Recommendations (Get personalized recommendations)
        public IActionResult Recommendations()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    var trending = _recommendationService.GetTrendingBooks(10);
                    ViewBag.Message = "Trending books for you:";
                    return View(trending);
                }

                var recommendations = _recommendationService.GetRecommendationsForUser(userId, 10);
                ViewBag.Message = "Personalized recommendations based on your reading history:";
                return View(recommendations);
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading recommendations.";
                _logger.LogError(ex, "Error loading recommendations.");
                return RedirectToAction("Index", "Home");
            }
        }

        // ADVANCED FEATURE #4: GET: /Chatbot/Similar/5 (Get similar books)
        public IActionResult Similar(int id)
        {
            try
            {
                var similarBooks = _recommendationService.GetSimilarBooks(id, 5);
                ViewBag.BookId = id;
                return View(similarBooks);
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading similar books.";
                _logger.LogError(ex, "Error loading similar books.");
                return RedirectToAction("Index", "Book");
            }
        }
    }
}