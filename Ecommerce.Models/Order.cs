using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Precision(15, 2)]
        public decimal ShippingCost { get; set; }
        public string DeliveryAddress { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentDetails { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public IEnumerable<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
