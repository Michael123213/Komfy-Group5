using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    // This model is used for transferring data between the WebApp and the Service Layer
    public class UserSettingModel
    {
        public int SettingID { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Theme is required.")]
        [StringLength(50)]
        public string Theme { get; set; } = "Light"; // Default theme

        // Navigation properties (for display purposes)
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }
}
