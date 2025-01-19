using Microsoft.AspNetCore.Mvc;
using TiendaOnline.Application.DTOs;

namespace TiendaOnline.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly HttpClient _httpClient;

        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.Identity.Name;
            var cart = await _httpClient.GetFromJsonAsync<CartDto>($"api/Cart/{userId}");
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userId = User.Identity.Name;
            var cartItemDto = new CartItemDto
            {
                ProductId = productId,
                Quantity = quantity
            };

            var response = await _httpClient.PostAsJsonAsync($"api/Cart/{userId}", cartItemDto);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Error al agregar el producto al carrito.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = User.Identity.Name;
            var response = await _httpClient.DeleteAsync($"api/Cart/{userId}/items/{productId}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Error al eliminar el producto del carrito.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.Identity.Name;
            var response = await _httpClient.DeleteAsync($"api/Cart/{userId}/clear");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Error al vaciar el carrito.";
            return RedirectToAction("Index");
        }
    }
}
