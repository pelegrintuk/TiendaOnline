using Microsoft.AspNetCore.Identity;
using TiendaOnline.Core.ValueObjects;

namespace TiendaOnline.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public Address Address { get; private set; }

        public void UpdateAddress(string street, string city, string state, string zipCode, string country)
        {
            Address = new Address(street, city, state, zipCode, country);
        }
    }
}
