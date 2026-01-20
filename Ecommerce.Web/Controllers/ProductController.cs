using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Ecommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Controllers
{
    public class ProductController(IUnitOfWork _unitOfWork, IWebHostEnvironment _environment) : Controller
    {
        public IActionResult Index()
        {
            var products = _unitOfWork.Products.Query()
                        .OrderByDescending(p => p.CreatedAt)
                        .Include(p => p.Category)
                        .Include(p => p.Brand)
                        .ToList();

            return View(products);
        }

        public IActionResult Create()
        {
            var productVM = new ProductVM
            {
                Brands = _unitOfWork.Brands.Query().ToList()
             .Select(b => new SelectListItem
             {
                 Value = b.Id.ToString(),
                 Text = b.Name
             }),

                Categories = _unitOfWork.Categories.Query().ToList()
             .Select(c => new SelectListItem
             {
                 Value = c.Id.ToString(),
                 Text = c.Name
             })
            };

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductVM productVM)
        {
            if (productVM.ImageFile is null)
            {
                ModelState.AddModelError("ImageFile", "uploading an image for the product is required");
            }

            if (!ModelState.IsValid)
            {
                productVM.Brands = _unitOfWork.Brands.Query().ToList()
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = b.Name
                    });

                productVM.Categories = _unitOfWork.Categories.Query().ToList()
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    });

                return View(productVM);
            }

            string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff")
                              + Path.GetExtension(productVM.ImageFile!.FileName);

            string filePath = Path.Combine(_environment.WebRootPath, "images/products", fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                productVM.ImageFile.CopyTo(stream);
            }

            Product newProduct = new()
            {
                Name = productVM.Name,
                Description = productVM.Description,
                Price = productVM.Price,
                ImageUrl = fileName,
                CategoryId = productVM.CategoryId,
                BrandId = productVM.BrandId,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Products.AddAsync(newProduct);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
