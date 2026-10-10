using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace market_mvc.Domain.Validation
{
    /// <summary>
    /// Requires a bool checkbox to be true. Unlike [Range(typeof(bool), "true", "true")],
    /// this works on both server and client: the jQuery unobtrusive "range" rule cannot
    /// compare booleans and always fails even when checked.
    /// Client adapter: market-mvc/wwwroot/js/shared.js (mustbetrue).
    /// </summary>
    public class MustBeTrueAttribute : ValidationAttribute, IClientModelValidator
    {
        public MustBeTrueAttribute()
            : base("You must agree to the terms and conditions")
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is true) return ValidationResult.Success;
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes["data-val"] = "true";
            context.Attributes["data-val-mustbetrue"] = FormatErrorMessage(
                context.ModelMetadata.GetDisplayName());
        }
    }
}
