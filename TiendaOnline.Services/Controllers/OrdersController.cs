using Microsoft.AspNetCore.Mvc;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Core.Entities;
using TiendaOnline.Application.DTOs;
using AutoMapper;

namespace TiendaOnline.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // No es necesario mapear aquí si ya se pasa un OrderDto
            await _orderService.CreateOrderAsync(orderDto);

            return CreatedAtAction(nameof(GetOrderById), new { id = orderDto.Id }, orderDto);
        }


        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] OrderDto orderDto)
        {
            if (orderId != orderDto.Id) return BadRequest();

            // Actualizar directamente con el DTO sin mapear a Order
            await _orderService.UpdateOrderAsync(orderDto);

            return NoContent();
        }


        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            await _orderService.DeleteOrderAsync(orderId);
            return NoContent();
        }
    }
}
