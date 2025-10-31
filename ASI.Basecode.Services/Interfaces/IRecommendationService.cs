using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IRecommendationService
    {
        // ADVANCED FEATURE #4: AI Chatbot for Book Recommendations

        // Get book recommendations based on preferences
        List<BookModel> GetRecommendationsByPreferences(string genre = null, string author = null, int minRating = 0);

        // Get recommendations based on a specific book (similar books)
        List<BookModel> GetSimilarBooks(int bookId, int count = 5);

        // Get recommendations based on user's borrowing history
        List<BookModel> GetRecommendationsForUser(string userId, int count = 10);

        // Get trending books (most popular recently)
        List<BookModel> GetTrendingBooks(int count = 10);

        // Process chatbot query and return recommendations
        ChatbotResponseModel ProcessChatbotQuery(string query, string userId = null);
    }
}