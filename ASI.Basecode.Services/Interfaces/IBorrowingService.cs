using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IBorrowingService
    {
        // READ operations
        List<BorrowingModel> GetAllBorrowings();
        BorrowingModel GetBorrowingDetails(int borrowingId);
        List<BorrowingModel> GetBorrowingsByUserId(string userId);
        List<BorrowingModel> GetBorrowingsByBookId(int bookId);
        List<BorrowingModel> GetActiveBorrowings();
        List<BorrowingModel> GetOverdueBorrowings();

        // CREATE operation
        void AddBorrowing(BorrowingModel model);

        // UPDATE operation
        void UpdateBorrowing(BorrowingModel model);
        void ReturnBook(int borrowingId);
        void MarkAsOverdue(int borrowingId);

        // DELETE operation
        void DeleteBorrowing(int borrowingId);
    }
}
