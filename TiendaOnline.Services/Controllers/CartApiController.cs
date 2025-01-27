using Microsoft.AspNetCore.Mvc;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace TiendaOnline.Services.Controllers
{
    [ApiController]
    [Route("api/Cart")]
    public class CartApiController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartApiController> _logger;

        public CartApiController(ICartService cartService, ILogger<CartApiController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> AddToCart(string userId, [FromBody] CartItemDto cartItemDto)
        {
            if (cartItemDto == null)
            {
                return BadRequest("Invalid cart item.");
            }

            _logger.LogInformation("Adding product {ProductId} to cart for user: {UserId}", cartItemDto.ProductId, userId);

            try
            {
                await _cartService.AddToCartAsync(userId, cartItemDto);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product {ProductId} to cart for user: {UserId}", cartItemDto.ProductId, userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(string userId)
        {
            _logger.LogInformation("Fetching cart for user: {UserId}", userId);

            try
            {
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cart for user: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{userId}/items/{productId}")]
        public async Task<IActionResult> RemoveFromCart(string userId, int productId)
        {
            _logger.LogInformation("Removing product {ProductId} from cart for user: {UserId}", productId, userId);

            try
            {
                await _cartService.RemoveFromCartAsync(userId, productId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing product {ProductId} from cart for user: {UserId}", productId, userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("Clear/{userId}")]
        public async Task<IActionResult> ClearCart(string userId)
        {
            _logger.LogInformation("Clearing cart for user: {UserId}", userId);

            try
            {
                await _cartService.ClearCartAsync(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for user: {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
