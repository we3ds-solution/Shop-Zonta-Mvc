using System.ComponentModel.DataAnnotations;
using market_mvc.Resources;

namespace market_mvc.Domain.ViewModels.Auth
{
    public class TwoFactorVM
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Code_Required")]
        [RegularExpression(@"^\d{6}$", ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Code_Regex")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Code_Display")]
        public string Code { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public class Setup2faVM
    {
        [Required]
        public string Secret { get; set; } = string.Empty;

        public string KeyUri { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Code_Required")]
        [RegularExpression(@"^\d{6}$", ErrorMessageResourceType = typeof(SharedValidation), ErrorMessageResourceName = "Code_Regex")]
        [Display(ResourceType = typeof(SharedValidation), Name = "Code_Display")]
        public string Code { get; set; } = string.Empty;

        public bool IsEnabled { get; set; }
    }
}
