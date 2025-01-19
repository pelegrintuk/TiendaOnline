﻿namespace TiendaOnline.Application.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public AddressDto Address { get; set; }
    }
}
