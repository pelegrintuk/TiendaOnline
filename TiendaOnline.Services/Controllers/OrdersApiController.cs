using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Application.Interfaces;

namespace TiendaOnline.Services.Controllers
{
    [ApiController]
    [Route("api/Orders")]
    public class OrdersApiController : ControllerBase
    {
        private readonly ILogger<OrdersApiController> _logger;
        private readonly ILogService _logService;

        public OrdersApiController(ILogger<OrdersApiController> logger, ILogService logService)
        {
            _logger = logger;
            _logService = logService;
        }

        [HttpPost("ProcessPayment")]
        public IActionResult ProcessPayment([FromBody] PaymentDto paymentDto)
        {
            _logger.LogInformation("Solicitud recibida en ProcessPayment");

            if (paymentDto == null)
            {
                var errorMessage = "El objeto PaymentDto es nulo";
                _logger.LogWarning(errorMessage);
                _logService.LogError(errorMessage, new Exception(errorMessage));
                return BadRequest("Datos de pago inválidos.");
            }

            _logger.LogInformation("Datos recibidos: {@PaymentDto}", paymentDto);

            // Validación del pago
            bool isValid = ValidatePayment(paymentDto);
            if (!isValid)
            {
                var errorMessage = "Datos de pago no válidos";
                _logger.LogWarning(errorMessage);
                _logService.LogError(errorMessage, new Exception(errorMessage));
                return BadRequest("Datos de pago inválidos.");
            }

            // Procesar el pago
            _logger.LogInformation("Pago procesado correctamente para la tarjeta: {CardNumber}", paymentDto.CardNumber);

            return Ok();
        }

        private bool ValidatePayment(PaymentDto paymentDto)
        {
            // Validar el número de tarjeta (algoritmo de Luhn)
            if (!IsValidCardNumber(paymentDto.CardNumber))
            {
                return false;
            }

            // Validar la fecha de expiración (formato MM/yyyy)
            if (!DateTime.TryParseExact(paymentDto.ExpiryDate, "MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var expiryDate) || expiryDate < DateTime.UtcNow)
            {
                return false;
            }

            // Validar el CVV (debe ser un número de 3 o 4 dígitos)
            if ((paymentDto.CVV.Length != 3 && paymentDto.CVV.Length != 4) || !int.TryParse(paymentDto.CVV, out _))
            {
                return false;
            }

            return true;
        }

        private bool IsValidCardNumber(string cardNumber)
        {
            // Remover espacios y guiones
            cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

            // Verificar que solo contenga dígitos
            if (!ulong.TryParse(cardNumber, out _))
            {
                return false;
            }

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
