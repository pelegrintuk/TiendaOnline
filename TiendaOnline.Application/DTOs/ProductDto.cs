using System.Collections.Generic;

namespace TiendaOnline.Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsFeatured { get; set; }
        public string Category { get; set; }
        public ICollection<string> Images { get; set; }  // Lista de URLs de imágenes
        public int Stock { get; set; } // Solo visible/modificable por administradores
    }
}
