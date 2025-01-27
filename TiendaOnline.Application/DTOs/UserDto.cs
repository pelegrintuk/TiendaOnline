using System.ComponentModel.DataAnnotations;

namespace TiendaOnline.Application.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string Email { get; set; }

        public AddressDto Address { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string Role { get; set; }
    }
}
