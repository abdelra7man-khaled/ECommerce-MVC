using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [Required]
        public int BrandId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem>? Brands { get; set; }
        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}
