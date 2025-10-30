using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Services.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;

        // Inject repositories
        public NotificationService(
            INotificationRepository notificationRepository,
            IUserRepository userRepository)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
        }

        public List<NotificationModel> GetAllNotifications()
        {
            var notifications = _notificationRepository.GetNotifications().ToList();

            // Mapping Data Model (Notification) to Service Model (NotificationModel)
            return notifications.Select(n => new NotificationModel
            {
                NotificationID = n.NotificationID,
                UserId = n.UserId,
                Message = n.Message,
                Timestamp = n.Timestamp,
                IsRead = n.IsRead,
                UserName = n.User?.Name,
                UserEmail = n.User?.Email
            }).ToList();
        }

        public NotificationModel GetNotificationDetails(int notificationId)
        {
            var notification = _notificationRepository.GetNotificationById(notificationId);

            if (notification == null)
            {
                return null;
            }

            return new NotificationModel
            {
                NotificationID = notification.NotificationID,
                UserId = notification.UserId,
                Message = notification.Message,
                Timestamp = notification.Timestamp,
                IsRead = notification.IsRead,
                UserName = notification.User?.Name,
                UserEmail = notification.User?.Email
            };
        }

        public List<NotificationModel> GetNotificationsByUserId(string userId)
        {
            var notifications = _notificationRepository.GetNotificationsByUserId(userId).ToList();

            return notifications.Select(n => new NotificationModel
            {
                NotificationID = n.NotificationID,
                UserId = n.UserId,
                Message = n.Message,
                Timestamp = n.Timestamp,
                IsRead = n.IsRead,
                UserName = n.User?.Name,
                UserEmail = n.User?.Email
            }).ToList();
        }

        public List<NotificationModel> GetUnreadNotificationsByUserId(string userId)
        {
            var notifications = _notificationRepository.GetUnreadNotificationsByUserId(userId).ToList();

            return notifications.Select(n => new NotificationModel
            {
                NotificationID = n.NotificationID,
                UserId = n.UserId,
                Message = n.Message,
                Timestamp = n.Timestamp,
                IsRead = n.IsRead,
                UserName = n.User?.Name,
                UserEmail = n.User?.Email
            }).ToList();
        }

        public int GetUnreadCountForUser(string userId)
        {
            return _notificationRepository.GetUnreadNotificationsByUserId(userId).Count();
        }

        public void AddNotification(NotificationModel model)
        {
            // Business Logic: Check if user exists
            if (!_userRepository.UserExists(model.UserId))
            {
                throw new Exception("User not found.");
            }

            // Mapping Service Model (NotificationModel) to Data Model (Notification)
            var notificationEntity = new Notification
            {
                UserId = model.UserId,
                Message = model.Message,
                Timestamp = model.Timestamp,
                IsRead = false // New notifications are always unread
            };

            _notificationRepository.AddNotification(notificationEntity);
        }

        public void UpdateNotification(NotificationModel model)
        {
            var notificationEntity = _notificationRepository.GetNotificationById(model.NotificationID);

            if (notificationEntity == null)
            {
                throw new KeyNotFoundException($"Notification with ID {model.NotificationID} not found.");
            }

            // Update fields
            notificationEntity.Message = model.Message;
            notificationEntity.IsRead = model.IsRead;

            _notificationRepository.UpdateNotification(notificationEntity);
        }

        public void MarkAsRead(int notificationId)
        {
            _notificationRepository.MarkAsRead(notificationId);
        }

        public void MarkAllAsReadForUser(string userId)
        {
            _notificationRepository.MarkAllAsReadForUser(userId);
        }

        public void DeleteNotification(int notificationId)
        {
            var notificationEntity = _notificationRepository.GetNotificationById(notificationId);

            if (notificationEntity == null)
            {
                throw new KeyNotFoundException($"Notification with ID {notificationId} not found.");
            }

            _notificationRepository.DeleteNotification(notificationEntity);
        }
    }
}
