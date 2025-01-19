using System.Collections.Generic;
using System.Threading.Tasks;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Core.Enums;

namespace TiendaOnline.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(string userId, List<OrderProductDto> orderProducts);
        Task<OrderDto> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);
    }
}
