using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.ViewModels
{
    public class PasswordResetVM
    {
        [Required(ErrorMessage = "Email is required"), EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "password field is required"), MaxLength(15)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "confirm password field is required")]
        [Compare("NewPassword", ErrorMessage = "confirm password and password not match")]
        public string ConfirmPassword { get; set; }
    }
}
