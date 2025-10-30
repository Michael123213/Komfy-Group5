using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ASI.Basecode.Data.Repositories
{
    public class NotificationRepository : BaseRepository, INotificationRepository
    {
        public NotificationRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Notification> GetNotifications()
        {
            return this.GetDbSet<Notification>()
                .Include(n => n.User)
                .OrderByDescending(n => n.Timestamp);
        }

        public Notification GetNotificationById(int notificationId)
        {
            return this.GetDbSet<Notification>()
                .Include(n => n.User)
                .FirstOrDefault(n => n.NotificationID == notificationId);
        }

        public IQueryable<Notification> GetNotificationsByUserId(string userId)
        {
            return this.GetDbSet<Notification>()
                .Include(n => n.User)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.Timestamp);
        }

        public IQueryable<Notification> GetUnreadNotificationsByUserId(string userId)
        {
            return this.GetDbSet<Notification>()
                .Include(n => n.User)
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.Timestamp);
        }

        public void AddNotification(Notification notification)
        {
            this.GetDbSet<Notification>().Add(notification);
            UnitOfWork.SaveChanges();
        }

        public void UpdateNotification(Notification notification)
        {
            this.SetEntityState(notification, EntityState.Modified);
            UnitOfWork.SaveChanges();
        }

        public void DeleteNotification(Notification notification)
        {
            this.SetEntityState(notification, EntityState.Deleted);
            UnitOfWork.SaveChanges();
        }

        public void MarkAsRead(int notificationId)
        {
            var notification = GetNotificationById(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                this.SetEntityState(notification, EntityState.Modified);
                UnitOfWork.SaveChanges();
            }
        }

        public void MarkAllAsReadForUser(string userId)
        {
            var notifications = this.GetDbSet<Notification>()
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToList();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                this.SetEntityState(notification, EntityState.Modified);
            }

            UnitOfWork.SaveChanges();
        }
    }
}