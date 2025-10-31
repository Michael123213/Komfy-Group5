using System.Collections.Generic;

namespace ASI.Basecode.Services.ServiceModels
{
    public class ChatbotResponseModel
    {
        public string Message { get; set; }
        public List<BookModel> RecommendedBooks { get; set; }
        public string QueryType { get; set; } // e.g., "genre", "author", "similar", "trending"

        public ChatbotResponseModel()
        {
            RecommendedBooks = new List<BookModel>();
        }
    }
}