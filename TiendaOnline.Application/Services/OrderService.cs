using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Core.Entities;
using TiendaOnline.Core.Enums;
using TiendaOnline.DAL.Data;

namespace TiendaOnline.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<OrderService> _logger;
        private readonly ILogService _logService;

        public OrderService(ApplicationDbContext context, IMapper mapper, IEmailService emailService, ILogger<OrderService> logger, ILogService logService)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
            _logService = logService;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
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

            // Verificar si el userId es nulo o vacío
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("UserId is null or empty");
                _logService.LogError("UserId is null or empty", new ArgumentException("UserId no puede ser nulo o vacío."));
                throw new ArgumentException("UserId no puede ser nulo o vacío.");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                // Crear un usuario temporal si no existe
                user = new ApplicationUser
                {
                    Id = userId,
                    UserName = "TempUser_" + userId,
                    Email = "tempuser@example.com",
                    Address = new Address // Asegurarse de que la dirección no sea nula
                    {
                        Street = "Temporal",
                        City = "Temporal",
                        State = "Temporal",
                        ZipCode = "00000",
                        Country = "Temporal"
                    }
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Temporary user created: {UserId}", userId);
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
                    var errorMessage = $"Product not available or insufficient stock: {orderProduct.ProductId}";
                    _logger.LogError(errorMessage);
                    _logService.LogError(errorMessage, new Exception(errorMessage));
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

            try
            {
                if (!string.IsNullOrEmpty(buyerEmail))
                {
                    await _emailService.SendEmailAsync(buyerEmail, subject, body);
                }
                else
                {
                    _logger.LogWarning("Buyer email is null or empty for user: {UserId}", userId);
                }

                if (!string.IsNullOrEmpty(sellerEmail))
                {
                    await _emailService.SendEmailAsync(sellerEmail, subject, body);
                }
                else
                {
                    _logger.LogWarning("Seller email is null or empty.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending confirmation email to user: {UserId}", userId);
                _logService.LogError("Error sending confirmation email to user", ex);
                throw;
            }

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
                var errorMessage = $"Order not found: {orderId}";
                _logger.LogWarning(errorMessage);
                _logService.LogError(errorMessage, new Exception(errorMessage));
                return null;
            }

            return _mapper.Map<OrderDto>(order);
        }

        public async Task UpdateOrderAsync(OrderDto orderDto)
        {
            var order = await _context.Orders.FindAsync(orderDto.OrderId);
            if (order == null)
            {
                var errorMessage = "La orden no existe.";
                _logService.LogError(errorMessage, new Exception(errorMessage));
                throw new Exception(errorMessage);
            }

            // Actualizar el estado
            order.Status = orderDto.Status;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderProducts) // Incluir OrderProducts para evitar problemas de eliminación
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}
