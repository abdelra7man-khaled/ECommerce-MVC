using Ecommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Web.Controllers
{
    public class CustomerOrderController(IUnitOfWork _unitOfWork, UserManager<IdentityUser> _userManager) : Controller
    {
        private const int PAGE_SIZE = 5;
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }


            var query = _unitOfWork.Orders.Query()
                        .Include(o => o.ApplicationUser)
                        .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                        .OrderByDescending(o => o.OrderDate)
                        .Where(o => o.ApplicationUserId == appUser.Id)
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
