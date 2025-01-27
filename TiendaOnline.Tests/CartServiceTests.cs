using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Application.Services;
using TiendaOnline.Core.Entities;
using TiendaOnline.DAL.Data;
using Xunit;
using MockQueryable.Moq;

namespace TiendaOnline.Tests
{
    public class CartServiceTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<CartService>> _loggerMock;
        private readonly CartService _cartService;

        public CartServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _contextMock = new Mock<ApplicationDbContext>(options);
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CartService>>();
            _cartService = new CartService(_contextMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetCartByUserIdAsync_ShouldReturnCart_WhenCartExists()
        {
            // Arrange
            var userId = "1";
            var cart = new Cart { UserId = userId, Items = new List<CartItem>() };
            var carts = new List<Cart> { cart }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Carts).Returns(carts.Object);
            _mapperMock.Setup(m => m.Map<CartDto>(It.IsAny<Cart>())).Returns(new CartDto());

            // Act
            var result = await _cartService.GetCartByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            _contextMock.Verify(c => c.Carts, Times.Once);
        }

        [Fact]
        public async Task AddToCartAsync_ShouldAddItemToCart_WhenCartExists()
        {
            // Arrange
            var userId = "1";
            var cartItemDto = new CartItemDto { ProductId = 1, Quantity = 1 };
            var cart = new Cart { UserId = userId, Items = new List<CartItem>() };
            var carts = new List<Cart> { cart }.AsQueryable().BuildMockDbSet();
            var product = new Product { ProductId = 1, Name = "Product1", Price = 10 };
            var products = new List<Product> { product }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Carts).Returns(carts.Object);
            _contextMock.Setup(c => c.Products).Returns(products.Object);

            // Act
            await _cartService.AddToCartAsync(userId, cartItemDto);

            // Assert
            _contextMock.Verify(c => c.Carts, Times.Once);
            _contextMock.Verify(c => c.Products, Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveFromCartAsync_ShouldRemoveItemFromCart_WhenItemExists()
        {
            // Arrange
            var userId = "1";
            var productId = 1;
            var cart = new Cart { UserId = userId, Items = new List<CartItem> { new CartItem { ProductId = productId } } };
            var carts = new List<Cart> { cart }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Carts).Returns(carts.Object);

            // Act
            await _cartService.RemoveFromCartAsync(userId, productId);

            // Assert
            _contextMock.Verify(c => c.Carts, Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ClearCartAsync_ShouldClearCart_WhenCartExists()
        {
            // Arrange
            var userId = "1";
            var cart = new Cart { UserId = userId, Items = new List<CartItem> { new CartItem { ProductId = 1 } } };
            var carts = new List<Cart> { cart }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Carts).Returns(carts.Object);

            // Act
            await _cartService.ClearCartAsync(userId);

            // Assert
            _contextMock.Verify(c => c.Carts, Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    }
}

