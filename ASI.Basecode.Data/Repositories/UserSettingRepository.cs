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

        public UserSetting GetSettingsByUserId(string userId)
        {
            return this.GetDbSet<UserSetting>().FirstOrDefault(s => s.UserId == userId);
        }

        public void AddUserSetting(UserSetting setting)
        {
            this.GetDbSet<UserSetting>().Add(setting);
            UnitOfWork.SaveChanges();
        }

        public void UpdateUserSetting(UserSetting setting)
        {
            this.SetEntityState(setting, EntityState.Modified);
            UnitOfWork.SaveChanges();
        }
    }
}