using System.ComponentModel.DataAnnotations;

namespace market_mvc.Domain.ObjectValues
{
    public class Money
    {
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "USD";
        
        public Money() { }
        
        public Money(decimal amount, string currency = "USD")
        {
            Amount = amount;
            Currency = currency;
        }
        
        public static Money Zero => new(0);
        
        public static Money operator +(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Cannot add money with different currencies");
                
            return new Money(left.Amount + right.Amount, left.Currency);
        }
        
        public static Money operator -(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Cannot subtract money with different currencies");
                
            return new Money(left.Amount - right.Amount, left.Currency);
        }
        
        public static Money operator *(Money money, decimal multiplier)
        {
            return new Money(money.Amount * multiplier, money.Currency);
        }
        
        public static bool operator >(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Cannot compare money with different currencies");
                
            return left.Amount > right.Amount;
        }
        
        public static bool operator <(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("Cannot compare money with different currencies");
                
            return left.Amount < right.Amount;
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is not Money other) return false;
            return Amount == other.Amount && Currency == other.Currency;
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(Amount, Currency);
        }
        
        public override string ToString()
        {
            return $"{Amount:C} {Currency}";
        }
    }
}

