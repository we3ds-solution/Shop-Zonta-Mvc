using FluentValidation;
using market_mvc.Features.Products.Commands.UpdateProduct;
using market_mvc.Infrastructure.Common;

namespace market_mvc.Features.Products.Validators
{
    public class UpdateProductCommandValidator : BaseValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Product ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.CompareAtPrice)
                .GreaterThan(x => x.Price).WithMessage("Compare at price must be greater than the regular price.")
                .When(x => x.CompareAtPrice.HasValue);

            RuleFor(x => x.CostPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Cost price must be greater than or equal to 0.")
                .When(x => x.CostPrice.HasValue);

            RuleFor(x => x.Sku)
                .MaximumLength(100).WithMessage("SKU cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Sku));

            RuleFor(x => x.Barcode)
                .MaximumLength(100).WithMessage("Barcode cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Barcode));

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be greater than or equal to 0.");

            RuleFor(x => x.LowStockThreshold)
                .GreaterThanOrEqualTo(0).WithMessage("Low stock threshold must be greater than or equal to 0.")
                .When(x => x.LowStockThreshold.HasValue);

            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than 0.")
                .When(x => x.Weight.HasValue);

            RuleFor(x => x.WeightUnit)
                .NotEmpty().WithMessage("Weight unit is required when weight is specified.")
                .When(x => x.Weight.HasValue);

            RuleFor(x => x.MetaTitle)
                .MaximumLength(200).WithMessage("Meta title cannot exceed 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.MetaTitle));

            RuleFor(x => x.MetaDescription)
                .MaximumLength(500).WithMessage("Meta description cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.MetaDescription));
        }
    }
}

