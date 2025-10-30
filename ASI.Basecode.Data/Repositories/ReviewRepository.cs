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
            return this.GetDbSet<Review>()
                .Include(r => r.User)
                .Include(r => r.Book);
        }

        public Review GetReviewById(int reviewId)
        {
            return this.GetDbSet<Review>()
                .Include(r => r.User)
                .Include(r => r.Book)
                .FirstOrDefault(r => r.ReviewID == reviewId);
        }

        public IQueryable<Review> GetReviewsByUserId(string userId)
        {
            return this.GetDbSet<Review>()
                .Include(r => r.User)
                .Include(r => r.Book)
                .Where(r => r.UserId == userId);
        }

        public IQueryable<Review> GetReviewsByBookId(int bookId)
        {
            return this.GetDbSet<Review>()
                .Include(r => r.User)
                .Include(r => r.Book)
                .Where(r => r.BookID == bookId);
        }

        public void AddReview(Review review)
        {
            this.GetDbSet<Review>().Add(review);
            UnitOfWork.SaveChanges();
        }

        public void UpdateReview(Review review)
        {
            this.SetEntityState(review, EntityState.Modified);
            UnitOfWork.SaveChanges();
        }

        public void DeleteReview(Review review)
        {
            this.SetEntityState(review, EntityState.Deleted);
            UnitOfWork.SaveChanges();
        }

        public bool UserHasReviewedBook(string userId, int bookId)
        {
            return this.GetDbSet<Review>().Any(r => r.UserId == userId && r.BookID == bookId);
        }
    }
}