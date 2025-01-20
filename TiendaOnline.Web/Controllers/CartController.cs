using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TiendaOnline.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace TiendaOnline.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CartController> _logger;

        public CartController(IHttpClientFactory httpClientFactory, ILogger<CartController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();

            _logger.LogInformation("Fetching cart for user: {UserId}", userId);

            try
            {
                var cart = await _httpClient.GetFromJsonAsync<CartDto>($"api/Cart/{userId}");
                return View(cart);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching cart for user: {UserId}", userId);
                TempData["ErrorMessage"] = "Error al cargar el carrito.";
                return View(new CartDto { Items = new List<CartItemDto>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userId = GetUserId();

            var cartItemDto = new CartItemDto
            {
                ProductId = productId,
                Quantity = quantity
            };

            _logger.LogInformation("Adding product {ProductId} to cart for user: {UserId}", productId, userId);

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/Cart/{userId}", cartItemDto);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Product {ProductId} added to cart for user: {UserId}", productId, userId);
                    return RedirectToAction("Index");
                }

                _logger.LogError("Error adding product {ProductId} to cart for user: {UserId}", productId, userId);
                TempData["ErrorMessage"] = "Error al agregar el producto al carrito.";
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error adding product {ProductId} to cart for user: {UserId}", productId, userId);
                TempData["ErrorMessage"] = "Error al agregar el producto al carrito.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = GetUserId();

            _logger.LogInformation("Removing product {ProductId} from cart for user: {UserId}", productId, userId);

            try
            {
                var response = await _httpClient.DeleteAsync($"api/Cart/{userId}/items/{productId}");
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Product {ProductId} removed from cart for user: {UserId}", productId, userId);
                    return RedirectToAction("Index");
                }

                _logger.LogError("Error removing product {ProductId} from cart for user: {UserId}", productId, userId);
                TempData["ErrorMessage"] = "Error al eliminar el producto del carrito.";
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error removing product {ProductId} from cart for user: {UserId}", productId, userId);
                TempData["ErrorMessage"] = "Error al eliminar el producto del carrito.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();

            _logger.LogInformation("Clearing cart for user: {UserId}", userId);

            try
            {
                var response = await _httpClient.DeleteAsync($"api/Cart/{userId}/clear");
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Cart cleared for user: {UserId}", userId);
                    return RedirectToAction("Index");
                }

                _logger.LogError("Error clearing cart for user: {UserId}", userId);
                TempData["ErrorMessage"] = "Error al vaciar el carrito.";
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error clearing cart for user: {UserId}", userId);
                TempData["ErrorMessage"] = "Error al vaciar el carrito.";
                return RedirectToAction("Index");
            }
        }

        private string GetUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                return User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            else
            {
                var tempUserId = HttpContext.Request.Cookies["TempUserId"];
                if (tempUserId == null)
                {
                    tempUserId = Guid.NewGuid().ToString();
                    HttpContext.Response.Cookies.Append("TempUserId", tempUserId, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(1) });
                }
                return tempUserId;
            }
        }
    }
}
