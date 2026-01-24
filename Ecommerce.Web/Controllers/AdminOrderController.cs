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

        public IActionResult Details(int id)
        {
            var order = _unitOfWork.Orders.Query()
                        .Include(o => o.ApplicationUser)
                        .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                        .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.NumberOfOrders = _unitOfWork.Orders.Query()
                                    .Where(o => o.ApplicationUserId == order.ApplicationUserId)
                                    .Count();

            return View(order);
        }

        public async Task<IActionResult> Edit(int id, string? payment_status, string? order_status)
        {
            var order = await _unitOfWork.Orders.GetAsync(id);
            if (order is null)
            {
                return RedirectToAction("Index");
            }
            if (string.IsNullOrEmpty(payment_status) && string.IsNullOrEmpty(order_status))
            {
                return RedirectToAction("Details", new { id });
            }

            if (!string.IsNullOrEmpty(payment_status))
            {
                order.PaymentStatus = payment_status;
                TempData["success"] = "Payment status updated successfully";
            }
            if (!string.IsNullOrEmpty(order_status))
            {
                order.OrderStatus = order_status;
                TempData["success"] = "Order status updated successfully";
            }

            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction("Details", new { id });
        }
    }
}
