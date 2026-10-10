using System.ComponentModel.DataAnnotations;

namespace market_mvc.Domain.ObjectValues
{
    public class Address
    {
        [Required]
        [StringLength(200)]
        public string Street { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string ZipCode { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;
        
        public string FullAddress => $"{Street}, {City}, {State} {ZipCode}, {Country}".Trim(' ', ',');
        
        public Address() { }
        
        public Address(string street, string city, string state, string zipCode, string country)
        {
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
        }
        
        public override bool Equals(object? obj)
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

