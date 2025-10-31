using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IReportingService
    {
        // ADVANCED FEATURE #2: Reporting System

        // Borrowing Report (which books borrowed, how long, return dates, unreturned)
        BorrowingReportModel GetBorrowingReport(string status = null);

        // Inventory Report (total books, filtered by genre/author/publisher)
        InventoryReportModel GetInventoryReport(string genre = null, string author = null, string publisher = null);

        // Member Report (registered members count)
        MemberReportModel GetMemberReport();
    }
}