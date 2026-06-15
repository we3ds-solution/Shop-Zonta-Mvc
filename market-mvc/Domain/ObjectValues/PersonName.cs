using System.ComponentModel.DataAnnotations;

namespace market_mvc.Domain.ObjectValues
{
    public class PersonName
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? MiddleName { get; set; }
        
        public string FullName => string.IsNullOrEmpty(MiddleName) 
            ? $"{FirstName} {LastName}" 
            : $"{FirstName} {MiddleName} {LastName}";
            
        public string DisplayName => $"{FirstName} {LastName}";
        
        public string LastNameFirst => string.IsNullOrEmpty(MiddleName)
            ? $"{LastName}, {FirstName}"
            : $"{LastName}, {FirstName} {MiddleName}";
        
        public PersonName() { }
        
        public PersonName(string firstName, string lastName, string? middleName = null)
        {
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is not PersonName other) return false;
            
            return FirstName == other.FirstName &&
                   LastName == other.LastName &&
                   MiddleName == other.MiddleName;
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(FirstName, LastName, MiddleName);
        }
        
        public override string ToString()
        {
            return FullName;
        }
    }
}

