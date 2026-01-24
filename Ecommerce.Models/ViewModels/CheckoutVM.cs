using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.ViewModels
{
    public class CheckoutVM
    {
        [Required(ErrorMessage = "The Delivery Address is required")]
        [MaxLength(200)]
        public string DeliveryAddress { get; set; }
        public string PaymentMethod { get; set; }
    }
}
