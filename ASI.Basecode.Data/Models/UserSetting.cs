// ASI.Basecode.Data.Models/UserSetting.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class UserSetting
    {
        public int SettingID { get; set; } // Primary Key

        // Foreign Key Property
        public string UserId { get; set; }

        public string Theme { get; set; } // e.g., 'Light' or 'Dark'

        // -----------------------------------------------------------------
        // NAVIGATION PROPERTIES
        // -----------------------------------------------------------------

        // USER_SETTINGS belongs to one User (One-to-One relationship)
        public virtual User User { get; set; }
    }
}