using Microsoft.AspNetCore.Mvc;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Core.Entities;
using TiendaOnline.Application.DTOs;
using AutoMapper;

namespace TiendaOnline.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrdersController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        [HttpPost("ProcessPayment")]
        public IActionResult ProcessPayment([FromBody] PaymentDto paymentDto)
        {
            if (ValidatePayment(paymentDto))
            {
                // Procesar el pago y devolver una respuesta exitosa
                return Ok();
            }

            return BadRequest("Datos de pago inválidos.");
        }

        private bool ValidatePayment(PaymentDto paymentDto)
        {
            // Validar el número de tarjeta (algoritmo de Luhn)
            if (!IsValidCardNumber(paymentDto.CardNumber))
            {
                return false;
            }

            // Validar la fecha de expiración
            if (!DateTime.TryParseExact(paymentDto.ExpiryDate, "yyyy-MM", null, System.Globalization.DateTimeStyles.None, out var expiryDate) || expiryDate < DateTime.UtcNow)
            {
                return false;
            }

            // Validar el CVV (debe ser un número de 3 dígitos)
            if (paymentDto.CVV.Length != 3 || !int.TryParse(paymentDto.CVV, out _))
            {
                return false;
            }

            return true;
        }

        private bool IsValidCardNumber(string cardNumber)
        {
            int sum = 0;
            bool alternate = false;
            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int n = int.Parse(cardNumber[i].ToString());
                if (alternate)
                {
                    n *= 2;
                    if (n > 9)
                    {
                        n -= 9;
                    }
                }
                sum += n;
                alternate = !alternate;
            }
            return (sum % 10 == 0);
        }
    }
}
