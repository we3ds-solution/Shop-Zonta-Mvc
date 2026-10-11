using System.ComponentModel.DataAnnotations;
using market_mvc.Domain.Validation;
using market_mvc.Resources;

namespace market_mvc.Domain.ViewModels.Auth
{
    /// <summary>
    /// ViewModel for user registration. Messages come from SharedValidation
    /// resources so they follow the current UI culture (EN/AR).
    /// </summary>
    public class RegisterVM
    {
        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "FullName_Required")]
        [StringLength(256, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "FullName_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "FullName_Display")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Email_Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Email_Invalid")]
        [StringLength(200, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Email_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Email_Display")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Password_Required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Password_Length")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Password_Regex")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Password_Display")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "ConfirmPassword_Required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "ConfirmPassword_Match")]
        [Display(ResourceType = typeof(SharedValidation), Name = "ConfirmPassword_Display")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(ResourceType = typeof(SharedValidation), Name = "Terms_Display")]
        [MustBeTrue]
        public bool AgreeToTerms { get; set; }
    }
}
