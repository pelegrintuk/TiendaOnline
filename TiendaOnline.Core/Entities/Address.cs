namespace TiendaOnline.Core.Entities
{
    public class Address
    {
        // Propiedades
        public int Id { get; set; } // Clave primaria para EF Core

        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }

        // Constructor para inicializar la dirección
        public Address() { } // EF Core necesita un constructor vacío para mapear objetos

        public Address(string street, string city, string state, string zipCode, string country)
        {
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
        }

        // Método para actualizar la dirección
        public void Update(string street, string city, string state, string zipCode, string country)
        {
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
        }

        // Sobrescribir Equals y GetHashCode para comparar Value Objects
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (Address)obj;
            return Street == other.Street &&
                   City == other.City &&
                   State == other.State &&
                   ZipCode == other.ZipCode &&
                   Country == other.Country;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Street, City, State, ZipCode, Country);
        }
    }
}
