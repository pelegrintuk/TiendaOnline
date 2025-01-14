namespace TiendaOnline.Application.DTOs;

public class CartDto
{
    public string UserId { get; set; }
    public List<CartItemDto> Items { get; set; }
    public decimal TotalPrice => Items?.Sum(item => item.Price * item.Quantity) ?? 0;
}

public class CartItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
