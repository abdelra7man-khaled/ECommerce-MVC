using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models.ViewModels;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly decimal _shippingCost;

        public CartController(IUnitOfWork unitOfWork, IConfiguration configuration,
            UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _userManager = userManager;
            _shippingCost = _configuration.GetValue<decimal>("CartSettings:ShippingCost");
        }
        public async Task<IActionResult> Index()
        {
            var cartItems = await CartHelper.GetCartItems(Request, Response, _unitOfWork);
            var cartTotal = CartHelper.GetCartTotal(cartItems);

            ViewBag.CartItems = cartItems;
            ViewBag.ShippingCost = _shippingCost;
            ViewBag.CartTotal = cartTotal;
            ViewBag.GrandTotal = cartTotal + _shippingCost;


            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(CheckoutVM checkoutVM)
        {
            var cartItems = await CartHelper.GetCartItems(Request, Response, _unitOfWork);
            var cartTotal = CartHelper.GetCartTotal(cartItems);
            ViewBag.CartItems = cartItems;
            ViewBag.ShippingCost = _shippingCost;
            ViewBag.CartTotal = cartTotal;
            ViewBag.GrandTotal = cartTotal + _shippingCost;
            if (!ModelState.IsValid)
            {
                return View(checkoutVM);
            }

            if (cartItems.Count == 0)
            {
                ViewBag.ErrorMessage = "Your cart is empty";
                return View(checkoutVM);
            }

            TempData["DeleveryAddress"] = checkoutVM.DeliveryAddress;
            TempData["PaymentMethod"] = checkoutVM.PaymentMethod;

            return RedirectToAction("Confirm");
        }
    }
}
