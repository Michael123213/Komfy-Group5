using System.Collections.Generic;

namespace ASI.Basecode.Services.ServiceModels
{
    public class InventoryReportModel
    {
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int BorrowedBooks { get; set; }
        public int Ebooks { get; set; }

        // Breakdown by Genre
        public Dictionary<string, int> BooksByGenre { get; set; }

        // Breakdown by Author
        public Dictionary<string, int> BooksByAuthor { get; set; }

        // Breakdown by Publisher
        public Dictionary<string, int> BooksByPublisher { get; set; }

        public List<BookModel> Books { get; set; }

        public InventoryReportModel()
        {
            BooksByGenre = new Dictionary<string, int>();
            BooksByAuthor = new Dictionary<string, int>();
            BooksByPublisher = new Dictionary<string, int>();
            Books = new List<BookModel>();
        }
    }
}