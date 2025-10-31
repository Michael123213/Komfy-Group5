using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IAnalyticsService
    {
        // ADVANCED FEATURE #1: Analytics Dashboard

        // Most Borrowed Books (Top 10)
        List<BookModel> GetMostBorrowedBooks(int count = 10);

        // Most Viewed Books (Top 10)
        List<BookModel> GetMostViewedBooks(int count = 10);

        // Top Borrowers (Users with most borrowings)
        List<UserAnalyticsModel> GetTopBorrowers(int count = 10);

        // Top Rated Books (Highest average ratings)
        List<BookModel> GetTopRatedBooks(int count = 10);

        // Dashboard Summary Statistics
        DashboardSummaryModel GetDashboardSummary();
    }
}