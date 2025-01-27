namespace TiendaOnline.Core.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; }
        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
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
