using System.Threading.Tasks;
using AutoMapper;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Core.Entities;
using TiendaOnline.DAL;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.DAL.Data;
using Microsoft.Extensions.Logging;

namespace TiendaOnline.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;

        public CartService(ApplicationDbContext context, IMapper mapper, ILogger<CartService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CartDto> GetCartByUserIdAsync(string userId)
        {
            _logger.LogInformation("Obteniendo carrito para el usuario: {UserId}", userId);

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                _logger.LogWarning("No se encontró carrito para el usuario: {UserId}", userId);
                return new CartDto { Items = new List<CartItemDto>() };
            }

            var cartDto = _mapper.Map<CartDto>(cart);
            return cartDto;
        }

        public async Task AddToCartAsync(string userId, CartItemDto cartItemDto)
        {
            _logger.LogInformation("Agregando producto al carrito del usuario: {UserId}", userId);

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                _context.Carts.Add(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == cartItemDto.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += cartItemDto.Quantity;
            }
            else
            {
                // Obtener el producto de la base de datos
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductId == cartItemDto.ProductId);

                if (product == null)
                {
                    throw new Exception("El producto no existe.");
                }

                // Crear el CartItem y asignar ProductName y Price
                var cartItem = new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = cartItemDto.Quantity
                };

                cart.Items.Add(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(string userId, int productId)
        {
            _logger.LogInformation("Eliminando producto del carrito del usuario: {UserId}", userId);

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    cart.Items.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            _logger.LogInformation("Limpiando carrito del usuario: {UserId}", userId);

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                cart.Items.Clear();
                await _context.SaveChangesAsync();
            }
        }
    }
}
