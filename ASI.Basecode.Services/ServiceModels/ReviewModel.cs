using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    // This model is used for transferring data between the WebApp and the Service Layer
    public class ReviewModel
    {
        public int ReviewID { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Book ID is required.")]
        public int BookID { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string Comment { get; set; }

        [Required(ErrorMessage = "Review date is required.")]
        [DataType(DataType.Date)]
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        // Navigation properties (for display purposes)
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string BookTitle { get; set; }
        public string BookCode { get; set; }
    }
}
