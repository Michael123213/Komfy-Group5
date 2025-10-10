using ASI.Basecode.Data.Models;
using System.Linq;

namespace ASI.Basecode.Data.Interfaces
{
    public interface INotificationRepository
    {
        IQueryable<Notification> GetNotificationsByUserId(string userId);
        void AddNotification(Notification notification);
        void MarkAsRead(Notification notification);
    }
}