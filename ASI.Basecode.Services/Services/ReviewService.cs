using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Services.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;

        // Inject repositories
        public ReviewService(
            IReviewRepository reviewRepository,
            IBookRepository bookRepository,
            IUserRepository userRepository)
        {
            _reviewRepository = reviewRepository;
            _bookRepository = bookRepository;
            _userRepository = userRepository;
        }

        public List<ReviewModel> GetAllReviews()
        {
            var reviews = _reviewRepository.GetReviews().ToList();

            // Mapping Data Model (Review) to Service Model (ReviewModel)
            return reviews.Select(r => new ReviewModel
            {
                ReviewID = r.ReviewID,
                UserId = r.UserId,
                BookID = r.BookID,
                Rating = r.Rating,
                Comment = r.Comment,
                ReviewDate = r.ReviewDate,
                UserName = r.User?.Name,
                UserEmail = r.User?.Email,
                BookTitle = r.Book?.Title,
                BookCode = r.Book?.BookCode
            }).ToList();
        }

        public ReviewModel GetReviewDetails(int reviewId)
        {
            var review = _reviewRepository.GetReviewById(reviewId);

            if (review == null)
            {
                return null;
            }

            return new ReviewModel
            {
                ReviewID = review.ReviewID,
                UserId = review.UserId,
                BookID = review.BookID,
                Rating = review.Rating,
                Comment = review.Comment,
                ReviewDate = review.ReviewDate,
                UserName = review.User?.Name,
                UserEmail = review.User?.Email,
                BookTitle = review.Book?.Title,
                BookCode = review.Book?.BookCode
            };
        }

        public List<ReviewModel> GetReviewsByUserId(string userId)
        {
            var reviews = _reviewRepository.GetReviewsByUserId(userId).ToList();

            return reviews.Select(r => new ReviewModel
            {
                ReviewID = r.ReviewID,
                UserId = r.UserId,
                BookID = r.BookID,
                Rating = r.Rating,
                Comment = r.Comment,
                ReviewDate = r.ReviewDate,
                UserName = r.User?.Name,
                UserEmail = r.User?.Email,
                BookTitle = r.Book?.Title,
                BookCode = r.Book?.BookCode
            }).ToList();
        }

        public List<ReviewModel> GetReviewsByBookId(int bookId)
        {
            var reviews = _reviewRepository.GetReviewsByBookId(bookId).ToList();

            return reviews.Select(r => new ReviewModel
            {
                ReviewID = r.ReviewID,
                UserId = r.UserId,
                BookID = r.BookID,
                Rating = r.Rating,
                Comment = r.Comment,
                ReviewDate = r.ReviewDate,
                UserName = r.User?.Name,
                UserEmail = r.User?.Email,
                BookTitle = r.Book?.Title,
                BookCode = r.Book?.BookCode
            }).ToList();
        }

        public double GetAverageRatingForBook(int bookId)
        {
            var reviews = _reviewRepository.GetReviewsByBookId(bookId).ToList();

            if (reviews.Count == 0)
            {
                return 0;
            }

            return reviews.Average(r => r.Rating);
        }

        public void AddReview(ReviewModel model)
        {
            // Business Logic: Check if user exists
            if (!_userRepository.UserExists(model.UserId))
            {
                throw new Exception("User not found.");
            }

            // Business Logic: Check if book exists
            var book = _bookRepository.GetBookById(model.BookID);
            if (book == null)
            {
                throw new Exception("Book not found.");
            }

            // Business Logic: Check if user has already reviewed this book
            if (_reviewRepository.UserHasReviewedBook(model.UserId, model.BookID))
            {
                throw new Exception("You have already reviewed this book.");
            }

            // Mapping Service Model (ReviewModel) to Data Model (Review)
            var reviewEntity = new Review
            {
                UserId = model.UserId,
                BookID = model.BookID,
                Rating = model.Rating,
                Comment = model.Comment,
                ReviewDate = model.ReviewDate
            };

            _reviewRepository.AddReview(reviewEntity);
        }

        public void UpdateReview(ReviewModel model)
        {
            var reviewEntity = _reviewRepository.GetReviewById(model.ReviewID);

            if (reviewEntity == null)
            {
                throw new KeyNotFoundException($"Review with ID {model.ReviewID} not found.");
            }

            // Update fields
            reviewEntity.Rating = model.Rating;
            reviewEntity.Comment = model.Comment;
            reviewEntity.ReviewDate = model.ReviewDate;

            _reviewRepository.UpdateReview(reviewEntity);
        }

        public void DeleteReview(int reviewId)
        {
            var reviewEntity = _reviewRepository.GetReviewById(reviewId);

            if (reviewEntity == null)
            {
                throw new KeyNotFoundException($"Review with ID {reviewId} not found.");
            }

            _reviewRepository.DeleteReview(reviewEntity);
        }
    }
}
