using System.ComponentModel.DataAnnotations;
using market_mvc.Resources;

namespace market_mvc.Domain.ViewModels.Auth
{
    /// <summary>
    /// ViewModel for user login. Messages follow the current UI culture (EN/AR).
    /// </summary>
    public class LoginVM
    {
        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Email_Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Email_Invalid")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Email_Display")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Password_Required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "LoginPassword_Length")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Password_Display")]
        public string Password { get; set; } = string.Empty;

        [Display(ResourceType = typeof(SharedValidation), Name = "RememberMe_Display")]
        public bool RememberMe { get; set; }
    }
}
