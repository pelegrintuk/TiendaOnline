using Microsoft.AspNetCore.Identity;


namespace TiendaOnline.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public Address Address { get; set; }

        public void UpdateAddress(string street, string city, string state, string zipCode, string country)
        {
            if (Address == null)
            {
                Address = new Address();
            }

            Address.Street = street;
            Address.City = city;
            Address.State = state;
            Address.ZipCode = zipCode;
            Address.Country = country;
        }
    }
}
