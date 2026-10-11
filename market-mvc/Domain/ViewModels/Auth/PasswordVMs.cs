using System.ComponentModel.DataAnnotations;
using market_mvc.Resources;

namespace market_mvc.Domain.ViewModels.Auth
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Email_Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Email_Invalid")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Email_Display")]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordVM
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

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
    }
}
