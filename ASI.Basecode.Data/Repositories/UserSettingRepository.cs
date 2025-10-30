using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ASI.Basecode.Data.Repositories
{
    public class UserSettingRepository : BaseRepository, IUserSettingRepository
    {
        public UserSettingRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<UserSetting> GetUserSettings()
        {
            return this.GetDbSet<UserSetting>()
                .Include(s => s.User);
        }

        public UserSetting GetUserSettingById(int settingId)
        {
            return this.GetDbSet<UserSetting>()
                .Include(s => s.User)
                .FirstOrDefault(s => s.SettingID == settingId);
        }

        public UserSetting GetUserSettingByUserId(string userId)
        {
            return this.GetDbSet<UserSetting>()
                .Include(s => s.User)
                .FirstOrDefault(s => s.UserId == userId);
        }

        public void AddUserSetting(UserSetting userSetting)
        {
            this.GetDbSet<UserSetting>().Add(userSetting);
            UnitOfWork.SaveChanges();
        }

        public void UpdateUserSetting(UserSetting userSetting)
        {
            this.SetEntityState(userSetting, EntityState.Modified);
            UnitOfWork.SaveChanges();
        }

        public void DeleteUserSetting(UserSetting userSetting)
        {
            this.SetEntityState(userSetting, EntityState.Deleted);
            UnitOfWork.SaveChanges();
        }

        public bool UserHasSetting(string userId)
        {
            return this.GetDbSet<UserSetting>().Any(s => s.UserId == userId);
        }
    }
}