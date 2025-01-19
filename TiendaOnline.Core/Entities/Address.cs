using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaOnline.Core.Entities
{
    public class Address
    {
        // Propiedades
        public int Id { get; set; }
        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string ZipCode { get; private set; }
        public string Country { get; private set; }

        // Constructor para inicializar la dirección
        private Address() { } // EF Core necesita un constructor vacío para mapear objetos

        public Address(string street, string city, string state, string zipCode, string country)
        {
            // Validar los datos antes de asignarlos
            if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException("Street is required.");
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City is required.");
            if (string.IsNullOrWhiteSpace(state)) throw new ArgumentException("State is required.");
            if (string.IsNullOrWhiteSpace(zipCode)) throw new ArgumentException("ZipCode is required.");
            if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country is required.");

            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
        }

        // Método para actualizar la dirección (crea una nueva instancia)
        public Address Update(string street, string city, string state, string zipCode, string country)
        {
            return new Address(street, city, state, zipCode, country);
        }

        // Sobrescribir Equals y GetHashCode para comparar Value Objects
        public override bool Equals(object obj)
        {
            if (obj is not Address other) return false;
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
