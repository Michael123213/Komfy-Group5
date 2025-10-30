using ASI.Basecode.Data.Models;
using System.Linq;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IReviewRepository
    {
        IQueryable<Review> GetReviews();
        Review GetReviewById(int reviewId);
        IQueryable<Review> GetReviewsByUserId(string userId);
        IQueryable<Review> GetReviewsByBookId(int bookId);
        void AddReview(Review review);
        void UpdateReview(Review review);
        void DeleteReview(Review review);
        bool UserHasReviewedBook(string userId, int bookId);
    }
}