namespace ASI.Basecode.Services.ServiceModels
{
    public class DashboardSummaryModel
    {
        // Book Statistics
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int BorrowedBooks { get; set; }
        public int EbooksCount { get; set; }

        // User Statistics
        public int TotalMembers { get; set; }
        public int TotalAdmins { get; set; }

        // Borrowing Statistics
        public int ActiveBorrowings { get; set; }
        public int OverdueBorrowings { get; set; }
        public int TotalBorrowingsAllTime { get; set; }

        // Review Statistics
        public int TotalReviews { get; set; }
        public double AverageRatingAcrossAllBooks { get; set; }
    }
}