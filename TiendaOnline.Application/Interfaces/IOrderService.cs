using TiendaOnline.Application.DTOs;

namespace TiendaOnline.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(); // Agregar esta línea
        Task<OrderDto> CreateOrderAsync(string userId, List<OrderProductDto> orderProducts);
        Task<OrderDto> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);
        Task UpdateOrderAsync(OrderDto orderDto); // Agregar esta línea
        Task DeleteOrderAsync(int orderId); // Agregar esta línea
    }
}
