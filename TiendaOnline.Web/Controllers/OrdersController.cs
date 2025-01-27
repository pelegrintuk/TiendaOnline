using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Checkout()
        {
            return View(new PaymentDto());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ProcessPayment(PaymentDto paymentDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Datos de pago inválidos.";
                return View("Checkout", paymentDto);
            }

            var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : Request.Cookies["TempUserId"];
            if (userId == null)
            {
                userId = Guid.NewGuid().ToString();
                Response.Cookies.Append("TempUserId", userId, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(1) });
            }
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

            try
            {
                var order = await _orderService.CreateOrderAsync(userId, orderProducts);

                // Limpiar el carrito
                await _cartService.ClearCartAsync(userId);

                // Procesar el pago
                var client = _httpClientFactory.CreateClient("ApiClient");
                var content = new StringContent(JsonSerializer.Serialize(paymentDto), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Orders/ProcessPayment", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Confirmation", new { orderId = order.OrderId });
                }

                _logger.LogError("Error processing payment for user: {UserId}", userId);
                TempData["ErrorMessage"] = "Error al procesar el pago.";
                return View("Checkout", paymentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for user: {UserId}", userId);
                TempData["ErrorMessage"] = "Error al procesar el pago.";
                return View("Checkout", paymentDto);
            }
        }

        public IActionResult Confirmation(int orderId)
        {
            var order = _orderService.GetOrderByIdAsync(orderId).Result;
            if (order == null)
            {
                TempData["ErrorMessage"] = "No se encontró el pedido.";
                return RedirectToAction("Index");
            }

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderDto orderDto)
        {
            if (id != orderDto.OrderId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _orderService.UpdateOrderAsync(orderDto);
                return RedirectToAction(nameof(Index));
            }

            // Si la validación falla, volvemos a cargar la orden completa
            var existingOrder = await _orderService.GetOrderByIdAsync(id);
            return View(existingOrder);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _orderService.DeleteOrderAsync(id);
                _logger.LogInformation("Order {OrderId} deleted by admin", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order {OrderId}", id);
                TempData["ErrorMessage"] = "Error al eliminar la orden.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
