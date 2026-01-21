using Ecommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Controllers
{
    public class HomeController(IUnitOfWork _unitOfWork) : Controller
    {
        public IActionResult Index()
        {
            var products = _unitOfWork.Products.Query()
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(4)
                    .ToList();

            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
