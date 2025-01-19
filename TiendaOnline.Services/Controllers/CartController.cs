using Microsoft.AspNetCore.Mvc;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Application.Interfaces;

namespace TiendaOnline.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCartByUser(string userId)
        {
            _logger.LogInformation("Fetching cart for user: {UserId}", userId);
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                _logger.LogWarning("Cart not found for user: {UserId}", userId);
                return NotFound();
            }
            return Ok(cart);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> AddToCart(string userId, [FromBody] CartItemDto cartItemDto)
        {
            _logger.LogInformation("Adding item to cart for user: {UserId}", userId);
            await _cartService.AddToCartAsync(userId, cartItemDto);
            return Ok("Item agregado al carrito");
        }

        [HttpDelete("{userId}/items/{itemId}")]
        public async Task<IActionResult> RemoveFromCart(string userId, int itemId)
        {
            _logger.LogInformation("Removing item from cart for user: {UserId}", userId);
            await _cartService.RemoveFromCartAsync(userId, itemId);
            return NoContent();
        }

        [HttpDelete("{userId}/clear")]
        public async Task<IActionResult> ClearCart(string userId)
        {
            _logger.LogInformation("Clearing cart for user: {UserId}", userId);
            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }
    }
}
