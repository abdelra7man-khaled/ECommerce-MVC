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

            TempData["success"] = "Product added successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var product = _unitOfWork.Products.Query()
                                .Include(p => p.Category)
                                .Include(p => p.Brand)
                                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                RedirectToAction(nameof(Index));
            }

            EditProductVM productVM = new()
            {
                Name = product!.Name,
                Description = product.Description,
                Price = product.Price,
                Brand = product.Brand,
                Category = product.Category
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageUrl;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditProductVM productVM)
        {
            var product = _unitOfWork.Products.Query()
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {

                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageUrl;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

                return View(productVM);
            }

            string newFileName = product.ImageUrl;
            if (productVM.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff")
                                  + Path.GetExtension(productVM.ImageFile.FileName);
                string filePath = Path.Combine(_environment.WebRootPath, "images/products", newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    productVM.ImageFile.CopyTo(stream);
                }

                string oldFilePath = Path.Combine(_environment.WebRootPath, "images/products", product.ImageUrl);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            product.Name = productVM.Name;
            product.Description = productVM.Description;
            product.Price = productVM.Price;
            product.ImageUrl = newFileName;

            await _unitOfWork.SaveChangesAsync();

            TempData["success"] = "Product updated successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Products.GetAsync(id);
            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }

            string filePath = Path.Combine(_environment.WebRootPath, "images/products", product.ImageUrl);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            _unitOfWork.Products.Remove(product);
            await _unitOfWork.SaveChangesAsync();

            TempData["success"] = "Product deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
