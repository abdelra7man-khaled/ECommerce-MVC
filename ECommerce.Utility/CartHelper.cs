using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;

namespace Ecommerce.Utility
{
    public class CartHelper
    {
        public static Dictionary<int, int> GetCartDictionary(HttpRequest request, HttpResponse response)
        {
            string cookieValue = request.Cookies["shopping_cart"] ?? string.Empty;

            try
            {
                var cart = Encoding.UTF8.GetString(Convert.FromBase64String(cookieValue));
                var dictionary = JsonSerializer.Deserialize<Dictionary<int, int>>(cart);
                if (dictionary != null)
                {
                    return dictionary;
                }
            }
            catch (Exception)
            {

            }

            if (cookieValue.Length > 0)
            {
                response.Cookies.Delete("shopping_cart");
            }

            return new Dictionary<int, int>();
        }

        public static int GetCartSize(HttpRequest request, HttpResponse response)
        {
            var cartDictionary = GetCartDictionary(request, response);
            int size = 0;
            foreach (var item in cartDictionary)
            {
                size += item.Value;
            }
            return size;
        }

        public static async Task<List<OrderItem>> GetCartItems(HttpRequest request, HttpResponse response, IUnitOfWork unitOfWork)
        {
            var cartDictionary = GetCartDictionary(request, response);
            var orderItems = new List<OrderItem>();
            foreach (var item in cartDictionary)
            {
                int productId = item.Key;
                int quantity = item.Value;
                var product = await unitOfWork.Products.GetAsync(productId);

                if (product is null) continue;

                var orderItem = new OrderItem
                {
                    Product = product,
                    Quantity = quantity,
                    UnitPrice = product.Price
                };

                orderItems.Add(orderItem);
            }

            return orderItems;
        }

        public static decimal GetCartTotal(List<OrderItem> orderItems)
        {
            decimal total = 0;
            foreach (var item in orderItems)
            {
                total += item.UnitPrice * item.Quantity;
            }
            return total;
        }
    }
}
