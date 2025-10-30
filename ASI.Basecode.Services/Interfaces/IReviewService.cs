using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IReviewService
    {
        // READ operations
        List<ReviewModel> GetAllReviews();
        ReviewModel GetReviewDetails(int reviewId);
        List<ReviewModel> GetReviewsByUserId(string userId);
        List<ReviewModel> GetReviewsByBookId(int bookId);
        double GetAverageRatingForBook(int bookId);

        // CREATE operation
        void AddReview(ReviewModel model);

        // UPDATE operation
        void UpdateReview(ReviewModel model);

        // DELETE operation
        void DeleteReview(int reviewId);
    }
}
