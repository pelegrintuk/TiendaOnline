using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaOnline.Core.Entities
{
    public class Cart
    {
        public int Id { get; set; } // Identificador único del carrito
        public string UserId { get; set; } // Relaciona el carrito con un usuario
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>(); // Productos en el carrito

        public decimal Total => Items.Sum(item => item.Price * item.Quantity); // Total calculado
    }

    public class CartItem
    {
        public int Id { get; set; } // Identificador único del ítem
        public int ProductId { get; set; } // Identificador del producto
        public string ProductName { get; set; } // Nombre del producto
        public decimal Price { get; set; } // Precio del producto
        public int Quantity { get; set; } // Cantidad

        // Relación con Product
        public Product Product { get; set; }

        // Relación con Cart
        public int CartId { get; set; }
        public Cart Cart { get; set; }
    }
}

