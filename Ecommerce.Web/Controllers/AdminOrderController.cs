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
        public IActionResult Index()
        {
            var orders = _unitOfWork.Orders.Query()
                        .Include(o => o.ApplicationUser)
                        .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                        .OrderByDescending(o => o.OrderDate)
                        .ToList();

            ViewBag.Orders = orders;

            return View();
        }
    }
}
