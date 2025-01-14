using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Core.Entities;
using TiendaOnline.DAL;

namespace TiendaOnline.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly TiendaContext _context;
        private readonly IMapper _mapper;

        public ProductService(TiendaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(bool isAdmin = false)
        {
            var products = _context.Products
                .Include(p => p.Images)
                .AsQueryable();

            if (!isAdmin)
                products = products.Where(p => p.Stock > 0); // Ocultar productos sin stock a usuarios normales

            return _mapper.Map<IEnumerable<ProductDto>>(await products.ToListAsync());
        }

        public async Task<ProductDto> GetProductByIdAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category)
        {
            var products = await _context.Products
                .Include(p => p.Images)
                .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string query)
        {
            var products = await _context.Products
                .Include(p => p.Images)
                .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            p.Description.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Images)
                .Where(p => p.IsFeatured)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task CreateProductAsync(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
