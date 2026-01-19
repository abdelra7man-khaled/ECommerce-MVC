using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
