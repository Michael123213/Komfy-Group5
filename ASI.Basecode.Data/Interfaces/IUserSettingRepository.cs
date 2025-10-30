using ASI.Basecode.Data.Models;
using System.Linq;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IUserSettingRepository
    {
        IQueryable<UserSetting> GetUserSettings();
        UserSetting GetUserSettingById(int settingId);
        UserSetting GetUserSettingByUserId(string userId);
        void AddUserSetting(UserSetting userSetting);
        void UpdateUserSetting(UserSetting userSetting);
        void DeleteUserSetting(UserSetting userSetting);
        bool UserHasSetting(string userId);
    }
}