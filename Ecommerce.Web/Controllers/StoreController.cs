using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Controllers
{
    public class StoreController(IUnitOfWork _unitOfWork) : Controller
    {
        private const int PAGE_SIZE = 8;
        public IActionResult Index(int pageNumber = 1, string? search = null,
            string? brand = null, string? category = null, string? sort = null)
        {
            var query = _unitOfWork.Products.Query()
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .AsQueryable();

            // Filtering
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(p => p.Brand.Name == brand);
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.Name == category);
            }

            if (!string.IsNullOrEmpty(sort))
            {
                query = sort switch
                {
                    "price_asc" => query.OrderBy(p => p.Price), // Low -> High
                    "price_desc" => query.OrderByDescending(p => p.Price), // High -> Low
                    _ => query.OrderByDescending(p => p.CreatedAt) // Newest first
                };
            }

            int count = query.Count();
            int totalPages = (int)Math.Ceiling(count / (double)PAGE_SIZE);

            var products = query.Skip((pageNumber - 1) * PAGE_SIZE)
                                .Take(PAGE_SIZE)
                                .ToList();

            StoreSearchVM storeSearchVM = new()
            {
                Search = search,
                Brand = brand,
                Category = category,
                Sort = sort,
            };

            ViewBag.Products = products;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = totalPages;

            return View(storeSearchVM);
        }

        public IActionResult Details(int id)
        {
            var product = _unitOfWork.Products.Query()
                            .Include(p => p.Category)
                            .Include(p => p.Brand)
                            .FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

    }
}