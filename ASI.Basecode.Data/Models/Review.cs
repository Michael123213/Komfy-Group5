// ASI.Basecode.Data.Models/Review.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class Review
    {
        public int ReviewID { get; set; } // Primary Key

        // Foreign Key Properties
        public string UserId { get; set; }
        public int BookID { get; set; }

        public int Rating { get; set; } // e.g., 1 to 5
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }

        // -----------------------------------------------------------------
        // NAVIGATION PROPERTIES
        // -----------------------------------------------------------------

        // REVIEWS belongs to one User
        public virtual User User { get; set; }

        // REVIEWS belongs to one Book
        public virtual Book Book { get; set; }
    }
}