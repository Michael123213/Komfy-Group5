// ASI.Basecode.Data.Models/Notification.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class Notification
    {
        public int NotificationID { get; set; } // Primary Key

        // Foreign Key Property
        public string UserId { get; set; }

        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }

        // -----------------------------------------------------------------
        // NAVIGATION PROPERTIES
        // -----------------------------------------------------------------

        // NOTIFICATIONS belongs to one User
        public virtual User User { get; set; }
    }
}