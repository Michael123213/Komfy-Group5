using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class Borrowing
    {
        public int BorrowingID { get; set; } // PK

        // Foreign Key Properties
        public string UserId { get; set; }
        public int BookID { get; set; }

        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; } // Nullable because it might not be returned yet
        public string Status { get; set; } // e.g., 'Active', 'Returned', 'Overdue'

        // -----------------------------------------------------------------
        // NAVIGATION PROPERTIES (Allows access to the related object)
        // -----------------------------------------------------------------

        // User who initiated the borrowing
        public virtual User User { get; set; }

        // Book that was borrowed
        public virtual Book Book { get; set; }
    }
}
