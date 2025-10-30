using ASI.Basecode.Data.Models;
using System.Linq;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IBorrowingRepository
    {
        IQueryable<Borrowing> GetBorrowings();
        Borrowing GetBorrowingById(int borrowingId);
        IQueryable<Borrowing> GetBorrowingsByUserId(string userId);
        IQueryable<Borrowing> GetBorrowingsByBookId(int bookId);
        IQueryable<Borrowing> GetActiveBorrowings();
        IQueryable<Borrowing> GetOverdueBorrowings();
        void AddBorrowing(Borrowing borrowing);
        void UpdateBorrowing(Borrowing borrowing);
        void DeleteBorrowing(Borrowing borrowing);
    }
}