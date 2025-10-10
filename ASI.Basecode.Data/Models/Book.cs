using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class Book
    {
        public int BookID { get; set; } // PK
        public string Title { get; set; }
        public string BookCode { get; set; }
        public string Genre { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string Status { get; set; } // e.g., 'Available', 'Borrowed'

        // -----------------------------------------------------------------
        // NAVIGATION PROPERTIES
        // -----------------------------------------------------------------

        // BOOKS ||--|{ BORROWINGS : "is part of"
        public virtual ICollection<Borrowing> Borrowings { get; set; }

        // BOOKS ||--|{ REVIEWS : "receives"
        public virtual ICollection<Review> Reviews { get; set; }

        public Book()
        {
            Borrowings = new HashSet<Borrowing>();
            Reviews = new HashSet<Review>();
        }
    }
}
