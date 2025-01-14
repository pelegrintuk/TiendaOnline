using System.Threading.Tasks;
using AutoMapper;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Core.Entities;
using TiendaOnline.DAL;
using Microsoft.EntityFrameworkCore;

namespace TiendaOnline.Application.Services
{
    public class CartService : ICartService
    {
        private readonly TiendaContext _context;
        private readonly IMapper _mapper;

        public CartService(TiendaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CartDto> GetCartByUserIdAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return _mapper.Map<CartDto>(cart ?? new Cart { UserId = userId });
        }

        public async Task AddToCartAsync(string userId, CartItemDto cartItemDto)
        {
            var cart = await GetCartByUserIdAsync(userId);

            var cartEntity = _mapper.Map<Cart>(cart);

            var item = cartEntity.Items.FirstOrDefault(i => i.ProductId == cartItemDto.ProductId);
            if (item == null)
            {
                var product = await _context.Products.FindAsync(cartItemDto.ProductId);
                if (product != null)
                {
                    cartEntity.Items.Add(new CartItem
                    {
                        ProductId = cartItemDto.ProductId,
                        ProductName = product.Name,
                        Price = product.Price,
                        Quantity = cartItemDto.Quantity
                    });
                }
            }
            else
            {
                item.Quantity += cartItemDto.Quantity;
            }

            _context.Carts.Update(cartEntity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(string userId, int productId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            var cartEntity = _mapper.Map<Cart>(cart);

            var item = cartEntity.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cartEntity.Items.Remove(item);
                _context.Carts.Update(cartEntity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            var cartEntity = _mapper.Map<Cart>(cart);

            cartEntity.Items.Clear();
            _context.Carts.Update(cartEntity);
            await _context.SaveChangesAsync();
        }
    }
}
