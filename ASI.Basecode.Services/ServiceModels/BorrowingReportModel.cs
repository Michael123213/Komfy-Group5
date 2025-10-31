using System.Collections.Generic;

namespace ASI.Basecode.Services.ServiceModels
{
    public class BorrowingReportModel
    {
        public int TotalBorrowings { get; set; }
        public int ActiveBorrowings { get; set; }
        public int ReturnedBorrowings { get; set; }
        public int OverdueBorrowings { get; set; }

        public List<BorrowingModel> Borrowings { get; set; }

        public BorrowingReportModel()
        {
            Borrowings = new List<BorrowingModel>();
        }
    }
}