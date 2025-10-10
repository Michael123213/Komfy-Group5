using ASI.Basecode.Data.Models;
using System.Linq;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IReviewRepository
    {
        IQueryable<Review> GetReviews();
        IQueryable<Review> GetReviewsByBookId(int bookId);
        Review GetReviewById(int reviewId);
        void AddReview(Review review);
        void DeleteReview(Review review);
    }
}