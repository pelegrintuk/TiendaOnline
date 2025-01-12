namespace TiendaOnline.Web.Models
{
    public class CartItemViewModel
    {
        public int ProductId { get; set; }         // Identificador del producto
        public string ProductName { get; set; }    // Nombre del producto
        public decimal Price { get; set; }         // Precio del producto
        public int Quantity { get; set; }          // Cantidad en el carrito
    }
}
