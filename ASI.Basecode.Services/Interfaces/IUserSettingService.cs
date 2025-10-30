using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserSettingService
    {
        // READ operations
        List<UserSettingModel> GetAllUserSettings();
        UserSettingModel GetUserSettingDetails(int settingId);
        UserSettingModel GetUserSettingByUserId(string userId);

        // CREATE operation
        void AddUserSetting(UserSettingModel model);
        void CreateDefaultSettingForUser(string userId);

        // UPDATE operation
        void UpdateUserSetting(UserSettingModel model);
        void UpdateTheme(string userId, string theme);

        // DELETE operation
        void DeleteUserSetting(int settingId);
    }
}
