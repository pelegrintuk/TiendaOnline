using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Core.Entities;
using TiendaOnline.Core.Enums;
using TiendaOnline.DAL;
using TiendaOnline.DAL.Data;
using Microsoft.Extensions.Logging;

namespace TiendaOnline.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ApplicationDbContext context, IMapper mapper, IEmailService emailService, ILogger<OrderService> logger)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId)
        {
            _logger.LogInformation("Fetching orders for user: {UserId}", userId);
            var orders = await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> CreateOrderAsync(string userId, List<OrderProductDto> orderProducts)
        {
            _logger.LogInformation("Creating order for user: {UserId}", userId);
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogError("User not found: {UserId}", userId);
                throw new Exception("Usuario no encontrado.");
            }

            var order = new Order
            {
                UserId = userId,
                User = user, // Establecer el usuario
                Date = DateTime.UtcNow,
                Status = OrderStatus.InProcess,
                OrderProducts = new List<OrderProduct>()
            };

            foreach (var orderProduct in orderProducts)
            {
                var product = await _context.Products.FindAsync(orderProduct.ProductId);
                if (product == null || product.Stock < orderProduct.Quantity)
                {
                    _logger.LogError("Product not available or insufficient stock: {ProductId}", orderProduct.ProductId);
                    throw new Exception("Producto no disponible o sin stock suficiente.");
                }

                product.Stock -= orderProduct.Quantity;

                order.OrderProducts.Add(new OrderProduct
                {
                    ProductId = orderProduct.ProductId,
                    Quantity = orderProduct.Quantity,
                    Price = product.Price
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Enviar correos electrónicos
            var buyerEmail = user.Email;
            var sellerEmail = "vendedor@tiendaonline.com"; // Cambiar por el correo del vendedor
            var subject = "Confirmación de compra";
            var body = $"Gracias por tu compra. Tu pedido #{order.OrderId} ha sido procesado.";

            await _emailService.SendEmailAsync(buyerEmail, subject, body);
            await _emailService.SendEmailAsync(sellerEmail, subject, body);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId)
        {
            _logger.LogInformation("Fetching order by ID: {OrderId}", orderId);
            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                _logger.LogWarning("Order not found: {OrderId}", orderId);
                return null;
            }

            return _mapper.Map<OrderDto>(order);
        }
    }
}
