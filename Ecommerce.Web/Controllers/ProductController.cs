using Ecommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Controllers
{
    public class ProductController(IUnitOfWork _unitOfWork) : Controller
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
    }
}
