using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Brand
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
