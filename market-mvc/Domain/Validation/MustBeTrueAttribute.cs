using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Resources;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using market_mvc.Resources;

namespace market_mvc.Domain.Validation
{
    /// <summary>
    /// Requires a bool checkbox to be true. Unlike [Range(typeof(bool), "true", "true")],
    /// this works on both server and client: the jQuery unobtrusive "range" rule cannot
    /// compare booleans and always fails even when checked.
    /// Message follows the current UI culture (SharedValidation.Terms_Required).
    /// Client adapter: market-mvc/wwwroot/js/shared.js (mustbetrue).
    /// </summary>
    public class MustBeTrueAttribute : ValidationAttribute, IClientModelValidator
    {
        private static readonly ResourceManager Resources =
            new(typeof(SharedValidation).FullName!, typeof(SharedValidation).Assembly);

        public MustBeTrueAttribute()
            : base(() => MessageFor(CultureInfo.CurrentUICulture))
        {
        }

        private static string MessageFor(CultureInfo culture) =>
            Resources.GetString("Terms_Required", culture) ?? "You must agree to the terms and conditions";

        public override string FormatErrorMessage(string name) =>
            MessageFor(CultureInfo.CurrentUICulture);

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is true) return ValidationResult.Success;
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes["data-val"] = "true";
            // Client message follows server culture at render time (page is per-culture).
            context.Attributes["data-val-mustbetrue"] = MessageFor(CultureInfo.CurrentUICulture);
        }
    }
}
