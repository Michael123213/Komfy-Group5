using ASI.Basecode.Data.Models;
using System.Linq;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IUserSettingRepository
    {
        UserSetting GetSettingsByUserId(string userId);
        void AddUserSetting(UserSetting setting);
        void UpdateUserSetting(UserSetting setting);
    }
}