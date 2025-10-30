using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    // This model is used for transferring data between the WebApp and the Service Layer
    public class BorrowingModel
    {
        public int BorrowingID { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Book ID is required.")]
        public int BookID { get; set; }

        [Required(ErrorMessage = "Borrow date is required.")]
        [DataType(DataType.Date)]
        public DateTime BorrowDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Due date is required.")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(50)]
        public string Status { get; set; } = "Active"; // Default status

        // Navigation properties (for display purposes)
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string BookTitle { get; set; }
        public string BookCode { get; set; }
    }
}
