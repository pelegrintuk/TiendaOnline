using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Core.Entities;
using TiendaOnline.DAL.Data;

namespace TiendaOnline.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ProductService(ApplicationDbContext context, IMapper mapper, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Images)
                .Where(p => p.IsFeatured)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(bool isAdmin = false)
        {
            var products = _context.Products
                .Include(p => p.Images)
                .AsQueryable();

            // Eliminar la condición que oculta productos sin stock
            // if (!isAdmin)
            //     products = products.Where(p => p.Stock > 0);

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
                .Where(p => p.Category.ToLower() == category.ToLower())
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string query)
        {
            var products = await _context.Products
                .Include(p => p.Images)
                .Where(p => p.Name.ToLower().Contains(query.ToLower()) ||
                            p.Description.ToLower().Contains(query.ToLower()))
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task CreateProductAsync(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);

            // Añadir imágenes si se proporcionan
            if (productDto.ImageFiles != null && productDto.ImageFiles.Any())
            {
                foreach (var file in productDto.ImageFiles)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    product.Images.Add(new ProductImage
                    {
                        ImageUrl = $"/images/{uniqueFileName}"
                    });
                }
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(ProductDto productDto)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == productDto.Id);

            if (product == null)
            {
                throw new Exception("Producto no encontrado.");
            }

            // Actualizar las propiedades del producto
            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.Category = productDto.Category;
            product.Stock = productDto.Stock;
            product.IsFeatured = productDto.IsFeatured;

            // Eliminar imágenes seleccionadas
            if (productDto.ImagesToDelete != null && productDto.ImagesToDelete.Any())
            {
                var imagesToDelete = product.Images.Where(i => productDto.ImagesToDelete.Contains(i.Id)).ToList();
                foreach (var image in imagesToDelete)
                {
                    // Eliminar el archivo físico de la imagen
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    product.Images.Remove(image);
                }
            }

            // Agregar nuevas imágenes si se subieron
            if (productDto.ImageFiles != null && productDto.ImageFiles.Any())
            {
                foreach (var file in productDto.ImageFiles)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    product.Images.Add(new ProductImage
                    {
                        ImageUrl = $"/images/{uniqueFileName}"
                    });
                }
            }

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product != null)
            {
                // Eliminar archivos físicos de imágenes
                foreach (var image in product.Images)
                {
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.ProductImages.RemoveRange(product.Images);
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
