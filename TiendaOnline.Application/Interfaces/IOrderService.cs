using System.Collections.Generic;
using System.Threading.Tasks;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Core.Enums;

namespace TiendaOnline.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto> GetOrderByIdAsync(int orderId);
        Task CreateOrderAsync(OrderDto orderDto);
        Task UpdateOrderAsync(OrderDto orderDto);
        Task DeleteOrderAsync(int orderId);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
    }
}
