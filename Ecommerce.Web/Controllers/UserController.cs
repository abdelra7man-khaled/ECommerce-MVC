using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private const int PAGE_SIZE = 5;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork _uow)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = _uow;
        }

        public IActionResult Index(int pageNumber = 1)
        {
            var query = _unitOfWork.ApplicationUsers.Query()
                            .OrderByDescending(u => u.CreatedAt)
                            .AsQueryable();

            int count = query.Count();
            int totalPages = (int)Math.Ceiling(count / (double)PAGE_SIZE);

            var appUsers = query.Skip((pageNumber - 1) * PAGE_SIZE)
                    .Take(PAGE_SIZE)
                    .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.PageNumber = pageNumber;

            return View(appUsers);
        }
    }
}
