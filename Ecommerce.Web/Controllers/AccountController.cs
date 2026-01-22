using Ecommerce.Models;
using Ecommerce.Models.ViewModels;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

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
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // display general errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(registerVM);
        }

        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var result = await _signInManager.PasswordSignInAsync(loginVM.Email,
                loginVM.Password, loginVM.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid Credentials, Please try again.";
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            if (_signInManager.IsSignedIn(User))
            {
                await _signInManager.SignOutAsync();
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var appUser = await _userManager.GetUserAsync(User) as ApplicationUser;
            if (appUser is null)
            {
                RedirectToAction("Index", "Home");
            }

            ProfileVM profileVM = new()
            {
                FirstName = appUser!.FirstName,
                LastName = appUser!.LastName,
                Email = appUser.Email ?? string.Empty,
                PhoneNumber = appUser.PhoneNumber ?? string.Empty,
                City = appUser!.City,
                Address = appUser!.Address
            };

            return View(profileVM);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileVM profileVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "please fill all fields are required";
                return View(profileVM);
            }

            var appUser = await _userManager.GetUserAsync(User) as ApplicationUser;
            if (appUser is null)
            {
                return RedirectToAction("Index", "Home");
            }

            appUser.FirstName = profileVM.FirstName;
            appUser.LastName = profileVM.LastName;
            appUser.Email = profileVM.Email;
            appUser.UserName = profileVM.Email;
            appUser.PhoneNumber = profileVM.PhoneNumber;
            appUser.City = profileVM.City;
            appUser.Address = profileVM.Address;

            var result = await _userManager.UpdateAsync(appUser);
            if (result.Succeeded)
            {
                TempData["success"] = "Profile updated successfully";
            }
            else
            {
                ViewBag.ErrorMessage = "failed to update profile: " + result.Errors.First().Description;
            }

            return View(profileVM);
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM changePasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var appUser = await _userManager.GetUserAsync(User) as ApplicationUser;
            if (appUser is null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ChangePasswordAsync(appUser,
                changePasswordVM.CurrentPassword, changePasswordVM.NewPassword);

            if (result.Succeeded)
            {
                TempData["success"] = "Password updated successfully";
            }
            else
            {
                ViewBag.ErrorMessage = "failed to update password: " + result.Errors.First().Description;
            }

            return View();
        }

        public IActionResult ForgotPassword()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([Required, EmailAddress] string email)
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Email = email;

            if (!ModelState.IsValid)
            {
                ViewBag.EmailError = ModelState["email"]?.Errors.First().ErrorMessage ?? "Invalid Email Address";
            }

            var appUser = await _userManager.FindByEmailAsync(email) as ApplicationUser;

            if (appUser != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                string resetUrl = Url.ActionLink("ResetPassword", "Account", new { token }) ?? "URL Error";

                Console.WriteLine("Password reset link: " + resetUrl);
            }

            ViewBag.SuccessMessage = "Please check your email and click on the password reset link";

            return View();
        }

        public IActionResult ResetPassword(string? token)
        {
            if (_signInManager.IsSignedIn(User) || token is null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string? token, PasswordResetVM passwordResetVM)
        {
            if (_signInManager.IsSignedIn(User) || token is null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(passwordResetVM);
            }

            var appUser = await _userManager.FindByEmailAsync(passwordResetVM.Email) as ApplicationUser;
            if (appUser == null)
            {
                ViewBag.ErrorMessage = "Token is not valid";
                return View(passwordResetVM);
            }

            var result = await _userManager.ResetPasswordAsync(appUser, token, passwordResetVM.NewPassword);
            if (result.Succeeded)
            {
                TempData["success"] = "Password reset successfully";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(passwordResetVM);
        }

        public IActionResult AccessDenied()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
