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
    }
}
