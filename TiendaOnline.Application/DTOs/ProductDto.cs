using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TiendaOnline.Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser un número positivo.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser un número no negativo.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public string Category { get; set; }

        public bool IsFeatured { get; set; }
        public ICollection<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();

        public List<IFormFile>? ImageFiles { get; set; } = new List<IFormFile>();

        public int[] ImagesToDelete { get; set; } = new int[0];
    }

    public class ProductImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
    }
}
