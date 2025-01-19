using System.Threading.Tasks;
using AutoMapper;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Core.Entities;
using TiendaOnline.DAL;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.DAL.Data;

namespace TiendaOnline.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CartService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CartDto> GetCartByUserIdAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return null;

            return _mapper.Map<CartDto>(cart);
        }

        public async Task AddToCartAsync(string userId, CartItemDto cartItemDto)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
            }

            var cartItem = cart.Items.FirstOrDefault(item => item.ProductId == cartItemDto.ProductId);
            if (cartItem == null)
            {
                cart.Items.Add(_mapper.Map<CartItem>(cartItemDto));
            }
            else
            {
                cartItem.Quantity += cartItemDto.Quantity;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(string userId, int itemId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return;

            var cartItem = cart.Items.FirstOrDefault(item => item.ProductId == itemId);
            if (cartItem != null)
            {
                cart.Items.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return;

            cart.Items.Clear();
            await _context.SaveChangesAsync();
        }
    }
}
