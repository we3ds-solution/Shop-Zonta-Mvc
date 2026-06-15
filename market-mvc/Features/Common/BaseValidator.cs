using FluentValidation;

namespace market_mvc.Infrastructure.Common
{
    public abstract class BaseValidator<T> : AbstractValidator<T>
    {
        protected BaseValidator()
        {
            // Common validation rules can be added here
        }
    }
}

