using System.ComponentModel.DataAnnotations;

namespace TiendaOnline.Application.DTOs
{
    public class PaymentDto
    {
        [Required(ErrorMessage = "El número de tarjeta es obligatorio.")]
        [CreditCard(ErrorMessage = "El número de tarjeta no es válido.")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "La fecha de expiración es obligatoria.")]
        [RegularExpression(@"^(0[1-9]|1[0-2])/20\d{2}$", ErrorMessage = "La fecha de expiración debe estar en formato MM/yyyy.")]
        public string ExpiryDate { get; set; }

        [Required(ErrorMessage = "El CVV es obligatorio.")]
        [StringLength(4, MinimumLength = 3, ErrorMessage = "El CVV debe tener 3 o 4 dígitos.")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "El CVV debe ser numérico.")]
        public string CVV { get; set; }
    }
}
