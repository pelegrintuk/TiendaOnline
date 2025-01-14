using System.Collections.Generic;

namespace TiendaOnline.Core.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; }
        public required ICollection<OrderProduct> OrderProducts { get; set; } // Relación con pedidos

        // Relación con imágenes
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public bool IsFeatured { get; set; }
    }

    public class ProductImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty; // URL de la imagen
        public int ProductId { get; set; }
        public Product Product { get; set; } // Relación con Product
    }
}
