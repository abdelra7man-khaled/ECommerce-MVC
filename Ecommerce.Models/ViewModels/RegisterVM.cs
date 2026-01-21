using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "first name field is required"), MaxLength(50)]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "last name field is required"), MaxLength(50)]
        public string LastName { get; set; }
        [Required(ErrorMessage = "email field is required"), EmailAddress, MaxLength(50)]
        public string Email { get; set; }
        [MaxLength(11)]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "address field is required"), MaxLength(200)]
        public string Address { get; set; }
        [Required(ErrorMessage = "city field is required"), MaxLength(100)]
        public string City { get; set; }
        [Required(ErrorMessage = "password field is required"), MaxLength(15)]
        public string Password { get; set; }
        [Required(ErrorMessage = "confirm password field is required")]
        [Compare("Password", ErrorMessage = "confirm password and password not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
