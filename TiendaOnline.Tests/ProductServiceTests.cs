using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Application.Services;
using TiendaOnline.Core.Entities;
using TiendaOnline.DAL.Data;
using Xunit;
using MockQueryable.Moq;
using Microsoft.AspNetCore.Hosting;

namespace TiendaOnline.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IHostingEnvironment> _hostingEnvironmentMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _contextMock = new Mock<ApplicationDbContext>(options);
            _mapperMock = new Mock<IMapper>();
            _hostingEnvironmentMock = new Mock<IHostingEnvironment>();
            _productService = new ProductService(_contextMock.Object, _mapperMock.Object, _hostingEnvironmentMock.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<Product>
                {
                    new Product { ProductId = 1, Name = "Product1", Price = 10, Stock = 10, Category = "Category1", Description = "Description1" },
                    new Product { ProductId = 2, Name = "Product2", Price = 20, Stock = 20, Category = "Category2", Description = "Description2" }
                }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Products).Returns(products.Object);
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>())).Returns(new List<ProductDto>());

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            Assert.NotNull(result);
            _contextMock.Verify(c => c.Products, Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var product = new Product { ProductId = 1, Name = "Product1", Price = 10, Stock = 10, Category = "Category1", Description = "Description1" };
            var products = new List<Product> { product }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Products).Returns(products.Object);
            _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns(new ProductDto());

            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            _contextMock.Verify(c => c.Products, Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            var products = new List<Product>().AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Products).Returns(products.Object);

            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            Assert.Null(result);
            _contextMock.Verify(c => c.Products, Times.Once);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldCreateProduct()
        {
            // Arrange
            var productDto = new ProductDto { Name = "Product1", Price = 10, Stock = 10, Category = "Category1", Description = "Description1" };
            var product = new Product { ProductId = 1, Name = "Product1", Price = 10, Stock = 10, Category = "Category1", Description = "Description1" };
            _mapperMock.Setup(m => m.Map<Product>(It.IsAny<ProductDto>())).Returns(product);
            _contextMock.Setup(c => c.Products.AddAsync(It.IsAny<Product>(), It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(new Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Product>(null));
            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(1);

            // Act
            await _productService.CreateProductAsync(productDto);

            // Assert
            _contextMock.Verify(c => c.Products.AddAsync(It.IsAny<Product>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldUpdateProduct_WhenProductExists()
        {
            // Arrange
            var productDto = new ProductDto { Id = 1, Name = "UpdatedProduct", Price = 20, Stock = 20, Category = "Category1", Description = "Description1" };
            var product = new Product { ProductId = 1, Name = "Product1", Price = 10, Stock = 10, Category = "Category1", Description = "Description1" };
            var products = new List<Product> { product }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Products).Returns(products.Object);
            _contextMock.Setup(c => c.Products.Update(It.IsAny<Product>())).Verifiable();
            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(1);

            // Act
            await _productService.UpdateProductAsync(productDto);

            // Assert
            _contextMock.Verify(c => c.Products.Update(It.IsAny<Product>()), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldThrowException_WhenProductDoesNotExist()
        {
            // Arrange
            var productDto = new ProductDto { Id = 1, Name = "UpdatedProduct", Price = 20, Stock = 20, Category = "Category1", Description = "Description1" };
            var products = new List<Product>().AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Products).Returns(products.Object);

            // Act & Assert
            await Assert.ThrowsAsync<System.Exception>(() => _productService.UpdateProductAsync(productDto));
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldDeleteProduct_WhenProductExists()
        {
            // Arrange
            var product = new Product { ProductId = 1, Name = "Product1", Price = 10, Stock = 10, Category = "Category1", Description = "Description1" };
            var products = new List<Product> { product }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Products).Returns(products.Object);
            _contextMock.Setup(c => c.Products.Remove(It.IsAny<Product>())).Verifiable();
            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(1);

            // Act
            await _productService.DeleteProductAsync(1);

            // Assert
            _contextMock.Verify(c => c.Products.Remove(It.IsAny<Product>()), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldThrowException_WhenProductDoesNotExist()
        {
            // Arrange
            var products = new List<Product>().AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Products).Returns(products.Object);

            // Act & Assert
            await Assert.ThrowsAsync<System.Exception>(() => _productService.DeleteProductAsync(1));
        }
    }
}
