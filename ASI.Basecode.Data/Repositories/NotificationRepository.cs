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

        public IQueryable<Notification> GetNotificationsByUserId(string userId)
        {
            // Note: Include logic to order by timestamp descending for typical notification views
            return this.GetDbSet<Notification>()
                       .Where(n => n.UserId == userId)
                       .OrderByDescending(n => n.Timestamp);
        }

        public void AddNotification(Notification notification)
        {
            this.GetDbSet<Notification>().Add(notification);
            UnitOfWork.SaveChanges();
        }

        public void MarkAsRead(Notification notification)
        {
            // Assuming you retrieve the entity first, then mark it as modified.
            // If the entity is attached, simply setting the property and saving changes works.
            this.SetEntityState(notification, EntityState.Modified);
            UnitOfWork.SaveChanges();
        }
    }
}