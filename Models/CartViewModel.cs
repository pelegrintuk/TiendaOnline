using System.Collections.Generic;

namespace TiendaOnline.Web.Models
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } // Lista de artículos en el carrito
        public decimal TotalPrice => Items?.Sum(item => item.Price * item.Quantity) ?? 0; // Precio total
    }
}
