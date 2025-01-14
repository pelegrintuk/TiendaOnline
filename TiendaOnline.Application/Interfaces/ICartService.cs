using System.Threading.Tasks;
using TiendaOnline.Application.DTOs;

namespace TiendaOnline.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartByUserIdAsync(string userId);
        Task AddToCartAsync(string userId, CartItemDto cartItemDto);
        Task RemoveFromCartAsync(string userId, int productId);
        Task ClearCartAsync(string userId);
    }
}
