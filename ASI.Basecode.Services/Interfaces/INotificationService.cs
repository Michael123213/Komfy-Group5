using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface INotificationService
    {
        // READ operations
        List<NotificationModel> GetAllNotifications();
        NotificationModel GetNotificationDetails(int notificationId);
        List<NotificationModel> GetNotificationsByUserId(string userId);
        List<NotificationModel> GetUnreadNotificationsByUserId(string userId);
        int GetUnreadCountForUser(string userId);

        // CREATE operation
        void AddNotification(NotificationModel model);

        // UPDATE operation
        void UpdateNotification(NotificationModel model);
        void MarkAsRead(int notificationId);
        void MarkAllAsReadForUser(string userId);

        // DELETE operation
        void DeleteNotification(int notificationId);
    }
}
