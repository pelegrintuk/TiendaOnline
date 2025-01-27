namespace TiendaOnline.Application.DTOs;
using TiendaOnline.Core.Enums;

public class OrderDto
{
    public OrderDto()
    {
        OrderProducts = new List<OrderProductDto>();
    }

    public int OrderId { get; set; }
    public DateTime Date { get; set; }
    public OrderStatus Status { get; set; }
    public string UserId { get; set; }
    public List<OrderProductDto> OrderProducts { get; set; }

    // Propiedad calculada para el total del pedido
    public decimal Total => OrderProducts?.Sum(op => op.Price * op.Quantity) ?? 0m;
}
