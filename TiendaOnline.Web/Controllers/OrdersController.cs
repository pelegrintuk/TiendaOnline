using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Application.Interfaces;

namespace TiendaOnline.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ICartService cartService, IHttpClientFactory httpClientFactory, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _cartService = cartService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            return View(new PaymentDto());
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(PaymentDto paymentDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Datos de pago inválidos.";
                return View("Checkout", paymentDto);
            }

            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : Request.Cookies["TempUserId"];
            _logger.LogInformation("Processing payment for user: {UserId}", userId);

            var cart = await _cartService.GetCartByUserIdAsync(userId);

            if (cart == null || !cart.Items.Any())
            {
                TempData["ErrorMessage"] = "El carrito está vacío.";
                return RedirectToAction("Checkout");
            }

            // Crear el pedido
            var orderProducts = cart.Items.Select(item => new OrderProductDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList();

            var order = await _orderService.CreateOrderAsync(userId, orderProducts);

            // Limpiar el carrito
            await _cartService.ClearCartAsync(userId);

            // Procesar el pago
            var client = _httpClientFactory.CreateClient("ApiClient");
            var content = new StringContent(JsonSerializer.Serialize(paymentDto), Encoding.UTF8, "application/json");
            try
            {
                var response = await client.PostAsync("api/Orders/ProcessPayment", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Confirmation");
                }

                _logger.LogError("Error processing payment for user: {UserId}", userId);
                TempData["ErrorMessage"] = "Error al procesar el pago.";
                return View("Checkout", paymentDto);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error processing payment for user: {UserId}", userId);
                TempData["ErrorMessage"] = "Error al procesar el pago.";
                return View("Checkout", paymentDto);
            }
        }

        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
