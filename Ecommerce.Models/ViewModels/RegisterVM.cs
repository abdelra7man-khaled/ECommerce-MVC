using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "First name field is required"), MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name field is required"), MaxLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required"), EmailAddress, MaxLength(50)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone number is required"), MaxLength(11)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Address field is required"), MaxLength(200)]
        public string Address { get; set; }
        [Required(ErrorMessage = "City is required"), MaxLength(100)]
        public string City { get; set; }
        [Required(ErrorMessage = "password field is required"), MaxLength(15)]
        public string Password { get; set; }
        [Required(ErrorMessage = "confirm password field is required")]
        [Compare("Password", ErrorMessage = "confirm password and password not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
