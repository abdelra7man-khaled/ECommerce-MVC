using Ecommerce.Models;
using Ecommerce.Models.ViewModels;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            var newUser = new ApplicationUser()
            {
                UserName = registerVM.Email,
                Email = registerVM.Email,
                PhoneNumber = registerVM.PhoneNumber,
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
                Address = registerVM.Address,
                City = registerVM.City,
                CreatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, SD.Role_Customer);
                await _signInManager.SignInAsync(newUser, false);
                return RedirectToAction("Index", "Home");
            }

            // display general errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(registerVM);
        }
    }
}
