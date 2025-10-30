using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Controllers
{
    [AllowAnonymous] // Temporarily allow access to all users for development
    public class ReviewController : Controller
    {
        private readonly ILogger<ReviewController> _logger;
        private readonly IReviewService _reviewService;

        // Inject IReviewService
        public ReviewController(ILogger<ReviewController> logger, IReviewService reviewService)
        {
            _logger = logger;
            _reviewService = reviewService;
        }

        // GET: /Review/Index (READ: List all reviews)
        public IActionResult Index()
        {
            var reviews = _reviewService.GetAllReviews();
            return View(reviews);
        }

        // GET: /Review/Create (CREATE: Display form)
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Review/Create (CREATE: Process form submission)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ReviewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _reviewService.AddReview(model);
                    TempData["SuccessMessage"] = "Review added successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {
                    // Add model error if business logic fails
                    ModelState.AddModelError(string.Empty, $"Error adding review: {ex.Message}");
                    _logger.LogError(ex, "Error adding review.");
                }
            }
            return View(model);
        }

        // GET: /Review/Edit/{id} (UPDATE: Display form with existing data)
        public IActionResult Edit(int id)
        {
            var review = _reviewService.GetReviewDetails(id);
            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }

        // POST: /Review/Edit/{id} (UPDATE: Process form submission)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ReviewModel model)
        {
            if (id != model.ReviewID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _reviewService.UpdateReview(model);
                    TempData["SuccessMessage"] = "Review updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error updating review: {ex.Message}");
                    _logger.LogError(ex, "Error updating review.");
                }
            }
            return View(model);
        }

        // POST: /Review/Delete/{id} (DELETE: Process deletion)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _reviewService.DeleteReview(id);
                TempData["SuccessMessage"] = "Review deleted successfully.";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "Review not found or already deleted.";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting review: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Review/Details/{id} (READ: View review details)
        public IActionResult Details(int id)
        {
            var review = _reviewService.GetReviewDetails(id);
            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }

        // GET: /Review/UserReviews/{userId} (READ: List reviews by user)
        public IActionResult UserReviews(string userId)
        {
            var reviews = _reviewService.GetReviewsByUserId(userId);
            return View(reviews);
        }

        // GET: /Review/BookReviews/{bookId} (READ: List reviews for a book)
        public IActionResult BookReviews(int bookId)
        {
            var reviews = _reviewService.GetReviewsByBookId(bookId);
            ViewBag.AverageRating = _reviewService.GetAverageRatingForBook(bookId);
            return View(reviews);
        }
    }
}
