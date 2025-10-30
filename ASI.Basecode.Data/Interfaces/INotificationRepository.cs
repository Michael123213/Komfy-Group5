using ASI.Basecode.Data.Models;
using System.Linq;

namespace ASI.Basecode.Data.Interfaces
{
    public interface INotificationRepository
    {
        IQueryable<Notification> GetNotifications();
        Notification GetNotificationById(int notificationId);
        IQueryable<Notification> GetNotificationsByUserId(string userId);
        IQueryable<Notification> GetUnreadNotificationsByUserId(string userId);
        void AddNotification(Notification notification);
        void UpdateNotification(Notification notification);
        void DeleteNotification(Notification notification);
        void MarkAsRead(int notificationId);
        void MarkAllAsReadForUser(string userId);
    }
}