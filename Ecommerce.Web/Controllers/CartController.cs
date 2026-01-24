using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
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

        public async Task<IActionResult> Confirm()
        {
            var cartItems = await CartHelper.GetCartItems(Request, Response, _unitOfWork);
            var grantTotal = CartHelper.GetCartTotal(cartItems) + _shippingCost;

            int cartSize = 0;
            foreach (var item in cartItems)
            {
                cartSize += item.Quantity;
            }

            string deliveryAddress = TempData["DeleveryAddress"]?.ToString() ?? string.Empty;
            string paymentMethod = TempData["PaymentMethod"]?.ToString() ?? string.Empty;
            TempData.Keep();

            if (cartSize == 0 || string.IsNullOrEmpty(deliveryAddress) || string.IsNullOrEmpty(paymentMethod))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.DeliveryAddress = deliveryAddress;
            ViewBag.PaymentMethod = paymentMethod;
            ViewBag.GrantTotal = grantTotal;
            ViewBag.CartSize = cartSize;

            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Confirm(int dummy)
        {
            var cartItems = await CartHelper.GetCartItems(Request, Response, _unitOfWork);

            string deliveryAddress = TempData["DeleveryAddress"]?.ToString() ?? string.Empty;
            string paymentMethod = TempData["PaymentMethod"]?.ToString() ?? string.Empty;
            TempData.Keep();

            if (cartItems.Count == 0 || string.IsNullOrEmpty(deliveryAddress) || string.IsNullOrEmpty(paymentMethod))
            {
                return RedirectToAction("Index", "Home");
            }

            var appUser = await _userManager.GetUserAsync(User) as ApplicationUser;
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var order = new Order
            {
                ApplicationUserId = appUser.Id,
                OrderItems = cartItems,
                ShippingCost = _shippingCost,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = paymentMethod,
                PaymentStatus = "Pending",
                PaymentDetails = string.Empty,
                OrderStatus = "Pending",
                OrderDate = DateTime.Now
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // Clear the cart cookie
            Response.Cookies.Delete("shopping_cart");

            ViewBag.SuccessMessage = "Your order has been placed successfully";

            return View();
        }
    }
}
