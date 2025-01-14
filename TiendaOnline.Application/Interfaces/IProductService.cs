using System.Collections.Generic;
using System.Threading.Tasks;
using TiendaOnline.Application.DTOs;

namespace TiendaOnline.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync(bool isAdmin = false);
        Task<ProductDto> GetProductByIdAsync(int productId);
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category);
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string query);
        Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync();
        Task CreateProductAsync(ProductDto productDto);
        Task UpdateProductAsync(ProductDto productDto);
        Task DeleteProductAsync(int productId);
    }
}

