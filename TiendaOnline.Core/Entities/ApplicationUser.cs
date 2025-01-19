using Microsoft.AspNetCore.Identity;


namespace TiendaOnline.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public Address Address { get; set; }

        public void UpdateAddress(string street, string city, string state, string zipCode, string country)
        {
            Address = new Address(street, city, state, zipCode, country);
        }
    }
}
