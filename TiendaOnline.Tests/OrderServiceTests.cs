using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Application.Services;
using TiendaOnline.Core.Entities;
using TiendaOnline.Core.Enums;
using TiendaOnline.DAL.Data;
using Xunit;
using MockQueryable.Moq;

namespace TiendaOnline.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly Mock<ILogService> _logServiceMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _contextMock = new Mock<ApplicationDbContext>(options);
            _mapperMock = new Mock<IMapper>();
            _emailServiceMock = new Mock<IEmailService>();
            _loggerMock = new Mock<ILogger<OrderService>>();
            _logServiceMock = new Mock<ILogService>();
            _orderService = new OrderService(_contextMock.Object, _mapperMock.Object, _emailServiceMock.Object, _loggerMock.Object, _logServiceMock.Object);
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnAllOrders()
        {
            // Arrange
            var orders = new List<Order>
                {
                    new Order { OrderId = 1, UserId = "1", Date = DateTime.UtcNow, Status = OrderStatus.InProcess },
                    new Order { OrderId = 2, UserId = "2", Date = DateTime.UtcNow, Status = OrderStatus.Completed }
                }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Orders).Returns(orders.Object);
            _mapperMock.Setup(m => m.Map<IEnumerable<OrderDto>>(It.IsAny<IEnumerable<Order>>())).Returns(new List<OrderDto>());

            // Act
            var result = await _orderService.GetAllOrdersAsync();

            // Assert
            Assert.NotNull(result);
            _contextMock.Verify(c => c.Orders, Times.Once);
        }

        [Fact]
        public async Task GetOrdersByUserIdAsync_ShouldReturnOrders_WhenUserExists()
        {
            // Arrange
            var userId = "1";
            var orders = new List<Order>
                {
                    new Order { OrderId = 1, UserId = userId, Date = DateTime.UtcNow, Status = OrderStatus.InProcess }
                }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Orders).Returns(orders.Object);
            _mapperMock.Setup(m => m.Map<IEnumerable<OrderDto>>(It.IsAny<IEnumerable<Order>>())).Returns(new List<OrderDto>());

            // Act
            var result = await _orderService.GetOrdersByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            _contextMock.Verify(c => c.Orders, Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldCreateOrder_WhenUserExists()
        {
            // Arrange
            var userId = "1";
            var orderProducts = new List<OrderProductDto>
                {
                    new OrderProductDto { ProductId = 1, Quantity = 1, Price = 10 }
                };
            var user = new ApplicationUser { Id = userId, UserName = "User1", Email = "user1@example.com" };
            var product = new Product { ProductId = 1, Name = "Product1", Price = 10, Stock = 10 };
            _contextMock.Setup(c => c.Users.FindAsync(It.IsAny<string>())).ReturnsAsync(user);
            _contextMock.Setup(c => c.Products.FindAsync(It.IsAny<int>())).ReturnsAsync(product);
            _contextMock.Setup(c => c.Orders.Add(It.IsAny<Order>())).Verifiable();
            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _orderService.CreateOrderAsync(userId, orderProducts);

            // Assert
            Assert.NotNull(result);
            _contextMock.Verify(c => c.Users.FindAsync(userId), Times.Once);
            _contextMock.Verify(c => c.Products.FindAsync(It.IsAny<int>()), Times.Once);
            _contextMock.Verify(c => c.Orders.Add(It.IsAny<Order>()), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var orderId = 1;
            var order = new Order { OrderId = orderId, UserId = "1", Date = DateTime.UtcNow, Status = OrderStatus.InProcess };
            var orders = new List<Order> { order }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Orders).Returns(orders.Object);
            _mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<Order>())).Returns(new OrderDto());

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            _contextMock.Verify(c => c.Orders, Times.Once);
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldUpdateOrder_WhenOrderExists()
        {
            // Arrange
            var orderDto = new OrderDto { OrderId = 1, Status = OrderStatus.Completed };
            var order = new Order { OrderId = 1, UserId = "1", Date = DateTime.UtcNow, Status = OrderStatus.InProcess };
            var orders = new List<Order> { order }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Orders).Returns(orders.Object);
            _contextMock.Setup(c => c.Orders.Update(It.IsAny<Order>())).Verifiable();
            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(1);

            // Act
            await _orderService.UpdateOrderAsync(orderDto);

            // Assert
            _contextMock.Verify(c => c.Orders.Update(It.IsAny<Order>()), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldDeleteOrder_WhenOrderExists()
        {
            // Arrange
            var orderId = 1;
            var order = new Order { OrderId = orderId, UserId = "1", Date = DateTime.UtcNow, Status = OrderStatus.InProcess };
            var orders = new List<Order> { order }.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Orders).Returns(orders.Object);
            _contextMock.Setup(c => c.Orders.Remove(It.IsAny<Order>())).Verifiable();
            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(1);

            // Act
            await _orderService.DeleteOrderAsync(orderId);

            // Assert
            _contextMock.Verify(c => c.Orders.Remove(It.IsAny<Order>()), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    }
}


