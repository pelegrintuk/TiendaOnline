namespace TiendaOnline.Application.DTOs;

public class OrderDto
{
    public int OrderId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; }
    public string UserId { get; set; }
    public List<OrderProductDto> OrderProducts { get; set; }
}