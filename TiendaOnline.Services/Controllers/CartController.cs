using Microsoft.AspNetCore.Mvc;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Application.Interfaces;

namespace TiendaOnline.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCartByUser(string userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> AddToCart(string userId, [FromBody] CartItemDto cartItemDto)
        {
            await _cartService.AddToCartAsync(userId, cartItemDto);
            return Ok("Item agregado al carrito");
        }

        [HttpDelete("{userId}/items/{itemId}")]
        public async Task<IActionResult> RemoveFromCart(string userId, int itemId)
        {
            await _cartService.RemoveFromCartAsync(userId, itemId);
            return NoContent();
        }
    }
}
