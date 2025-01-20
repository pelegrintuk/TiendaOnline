using System.Collections.Generic;

namespace TiendaOnline.Application.DTOs
{
    public class UserProfileDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public AddressDto Address { get; set; }
        public IEnumerable<OrderDto> Orders { get; set; }
    }
}