using ASI.Basecode.Data.Models;
using System.Linq;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IBorrowingRepository
    {
        IQueryable<Borrowing> GetBorrowings();
        IQueryable<Borrowing> GetBorrowingsByUserId(string userId);
        Borrowing GetBorrowingById(int borrowingId);
        void AddBorrowing(Borrowing borrowing);
        void UpdateBorrowing(Borrowing borrowing);
    }
}