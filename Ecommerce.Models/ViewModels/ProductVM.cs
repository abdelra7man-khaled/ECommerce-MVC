using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace Ecommerce.Models.ViewModels
{
    public class ProductVM
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public IFormFile? ImageFile { get; set; }

        public Brand Brand { get; set; }
        public Category Category { get; set; }
    }
}
