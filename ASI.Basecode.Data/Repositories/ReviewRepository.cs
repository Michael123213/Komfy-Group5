using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ASI.Basecode.Data.Repositories
{
    public class ReviewRepository : BaseRepository, IReviewRepository
    {
        public ReviewRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Review> GetReviews()
        {
            return this.GetDbSet<Review>();
        }

        public IQueryable<Review> GetReviewsByBookId(int bookId)
        {
            return this.GetDbSet<Review>().Where(r => r.BookID == bookId);
        }

        public Review GetReviewById(int reviewId)
        {
            return this.GetDbSet<Review>().FirstOrDefault(r => r.ReviewID == reviewId);
        }

        public void AddReview(Review review)
        {
            this.GetDbSet<Review>().Add(review);
            UnitOfWork.SaveChanges();
        }

        public void DeleteReview(Review review)
        {
            this.SetEntityState(review, EntityState.Deleted);
            UnitOfWork.SaveChanges();
        }
    }
}