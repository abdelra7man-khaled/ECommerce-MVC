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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork _uow)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = _uow;
        }

        public IActionResult Index()
        {
            //var query = _userManager.Users as IQueryable<ApplicationUser>;
            //var appUsers = query?.OrderByDescending(u => u.CreatedAt).ToList();
            var appUsers = _unitOfWork.ApplicationUsers.Query()
                            .OrderByDescending(u => u.CreatedAt)
                            .ToList();

            return View(appUsers);
        }
    }
}
