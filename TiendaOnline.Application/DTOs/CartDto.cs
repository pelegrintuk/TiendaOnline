namespace TiendaOnline.Application.DTOs;

public class CartDto
{
    public string UserId { get; set; }
    public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();

    // Propiedad calculada para el total del carrito
    public decimal Total => Items.Sum(item => item.Price * item.Quantity);
}

public class CartItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    // Propiedad calculada para el precio total del ítem
    public decimal TotalPrice => Price * Quantity;
}
