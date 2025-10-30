using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Services.Services
{
    public class UserSettingService : IUserSettingService
    {
        private readonly IUserSettingRepository _userSettingRepository;
        private readonly IUserRepository _userRepository;

        // Inject repositories
        public UserSettingService(
            IUserSettingRepository userSettingRepository,
            IUserRepository userRepository)
        {
            _userSettingRepository = userSettingRepository;
            _userRepository = userRepository;
        }

        public List<UserSettingModel> GetAllUserSettings()
        {
            var userSettings = _userSettingRepository.GetUserSettings().ToList();

            // Mapping Data Model (UserSetting) to Service Model (UserSettingModel)
            return userSettings.Select(s => new UserSettingModel
            {
                SettingID = s.SettingID,
                UserId = s.UserId,
                Theme = s.Theme,
                UserName = s.User?.Name,
                UserEmail = s.User?.Email
            }).ToList();
        }

        public UserSettingModel GetUserSettingDetails(int settingId)
        {
            var userSetting = _userSettingRepository.GetUserSettingById(settingId);

            if (userSetting == null)
            {
                return null;
            }

            return new UserSettingModel
            {
                SettingID = userSetting.SettingID,
                UserId = userSetting.UserId,
                Theme = userSetting.Theme,
                UserName = userSetting.User?.Name,
                UserEmail = userSetting.User?.Email
            };
        }

        public UserSettingModel GetUserSettingByUserId(string userId)
        {
            var userSetting = _userSettingRepository.GetUserSettingByUserId(userId);

            if (userSetting == null)
            {
                return null;
            }

            return new UserSettingModel
            {
                SettingID = userSetting.SettingID,
                UserId = userSetting.UserId,
                Theme = userSetting.Theme,
                UserName = userSetting.User?.Name,
                UserEmail = userSetting.User?.Email
            };
        }

        public void AddUserSetting(UserSettingModel model)
        {
            // Business Logic: Check if user exists
            if (!_userRepository.UserExists(model.UserId))
            {
                throw new Exception("User not found.");
            }

            // Business Logic: Check if user already has settings
            if (_userSettingRepository.UserHasSetting(model.UserId))
            {
                throw new Exception("User already has settings. Use update instead.");
            }

            // Mapping Service Model (UserSettingModel) to Data Model (UserSetting)
            var userSettingEntity = new UserSetting
            {
                UserId = model.UserId,
                Theme = model.Theme
            };

            _userSettingRepository.AddUserSetting(userSettingEntity);
        }

        public void CreateDefaultSettingForUser(string userId)
        {
            // Business Logic: Check if user exists
            if (!_userRepository.UserExists(userId))
            {
                throw new Exception("User not found.");
            }

            // Business Logic: Check if user already has settings
            if (_userSettingRepository.UserHasSetting(userId))
            {
                return; // Settings already exist, no need to create
            }

            // Create default settings
            var defaultSetting = new UserSetting
            {
                UserId = userId,
                Theme = "Light" // Default theme
            };

            _userSettingRepository.AddUserSetting(defaultSetting);
        }

        public void UpdateUserSetting(UserSettingModel model)
        {
            var userSettingEntity = _userSettingRepository.GetUserSettingById(model.SettingID);

            if (userSettingEntity == null)
            {
                throw new KeyNotFoundException($"User setting with ID {model.SettingID} not found.");
            }

            // Update fields
            userSettingEntity.Theme = model.Theme;

            _userSettingRepository.UpdateUserSetting(userSettingEntity);
        }

        public void UpdateTheme(string userId, string theme)
        {
            var userSettingEntity = _userSettingRepository.GetUserSettingByUserId(userId);

            if (userSettingEntity == null)
            {
                // If settings don't exist, create them
                CreateDefaultSettingForUser(userId);
                userSettingEntity = _userSettingRepository.GetUserSettingByUserId(userId);
            }

            // Update theme
            userSettingEntity.Theme = theme;
            _userSettingRepository.UpdateUserSetting(userSettingEntity);
        }

        public void DeleteUserSetting(int settingId)
        {
            var userSettingEntity = _userSettingRepository.GetUserSettingById(settingId);

            if (userSettingEntity == null)
            {
                throw new KeyNotFoundException($"User setting with ID {settingId} not found.");
            }

            _userSettingRepository.DeleteUserSetting(userSettingEntity);
        }
    }
}
