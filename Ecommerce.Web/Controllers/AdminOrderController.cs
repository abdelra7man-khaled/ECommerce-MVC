using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminOrderController(IUnitOfWork _unitOfWork) : Controller
    {
        private const int PAGE_SIZE = 5;
        public IActionResult Index(int pageNumber = 1)
        {
            var query = _unitOfWork.Orders.Query()
                        .Include(o => o.ApplicationUser)
                        .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                        .OrderByDescending(o => o.OrderDate)
                        .AsQueryable();

            int count = query.Count();
            int totalPages = (int)Math.Ceiling(count / (double)PAGE_SIZE);
            var orders = query
                        .Skip((pageNumber - 1) * PAGE_SIZE)
                        .Take(PAGE_SIZE)
                        .ToList();


            ViewBag.Orders = orders;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = totalPages;

            return View();
        }
    }
}
