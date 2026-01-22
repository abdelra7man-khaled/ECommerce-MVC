using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.ViewModels
{
    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "Current Password field is required"), MaxLength(15)]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "New Password field is required"), MaxLength(15)]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "confirm password field is required")]
        [Compare("NewPassword", ErrorMessage = "confirm password and new password not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
