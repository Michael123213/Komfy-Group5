using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ASI.Basecode.Data.Repositories
{
    public class BorrowingRepository : BaseRepository, IBorrowingRepository
    {
        public BorrowingRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Borrowing> GetBorrowings()
        {
            return this.GetDbSet<Borrowing>();
        }

        public IQueryable<Borrowing> GetBorrowingsByUserId(string userId)
        {
            return this.GetDbSet<Borrowing>().Where(b => b.UserId == userId);
        }

        public Borrowing GetBorrowingById(int borrowingId)
        {
            return this.GetDbSet<Borrowing>().FirstOrDefault(b => b.BorrowingID == borrowingId);
        }

        public void AddBorrowing(Borrowing borrowing)
        {
            this.GetDbSet<Borrowing>().Add(borrowing);
            UnitOfWork.SaveChanges();
        }

        public void UpdateBorrowing(Borrowing borrowing)
        {
            this.SetEntityState(borrowing, EntityState.Modified);
            UnitOfWork.SaveChanges();
        }
    }
}