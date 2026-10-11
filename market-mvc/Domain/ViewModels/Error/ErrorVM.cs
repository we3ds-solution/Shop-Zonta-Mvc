using System.ComponentModel.DataAnnotations;

namespace market_mvc.Domain.ViewModels.Error
{
    public class ErrorVM
    {
        public string? RequestId { get; set; }
        
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        
        public string? ErrorMessage { get; set; }
        
        public string? ErrorTitle { get; set; }
        
        public int StatusCode { get; set; }
        
        public string? ExceptionMessage { get; set; }
        
        public string? StackTrace { get; set; }
        
        public DateTime ErrorTime { get; set; } = DateTime.UtcNow;
        
        public string? UserAgent { get; set; }
        
        public string? RequestPath { get; set; }
        
        public string? HttpMethod { get; set; }
        
        public bool ShowDetails { get; set; } = false;
        
        public string? Source { get; set; }
        
        public Dictionary<string, string> AdditionalData { get; set; } = new();
        
        // Helper properties
        public string DisplayTitle => !string.IsNullOrEmpty(ErrorTitle) ? ErrorTitle : GetDefaultTitle();
        
        public string DisplayMessage => !string.IsNullOrEmpty(ErrorMessage) ? ErrorMessage : GetDefaultMessage();
        
        public string FormattedErrorTime => ErrorTime.ToString("yyyy-MM-dd HH:mm:ss UTC");
        
        public bool IsClientError => StatusCode >= 400 && StatusCode < 500;
        
        public bool IsServerError => StatusCode >= 500;
        
        public string StatusCodeClass => StatusCode switch
        {
            >= 400 and < 500 => "text-warning",
            >= 500 => "text-danger",
            _ => "text-info"
        };
        
        public string IconClass => StatusCode switch
        {
            400 => "fas fa-exclamation-triangle",
            401 => "fas fa-lock",
            403 => "fas fa-ban",
            404 => "fas fa-search",
            500 => "fas fa-server",
            503 => "fas fa-tools",
            _ => "fas fa-exclamation-circle"
        };
        
        private static bool IsArabic =>
            System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar";

        private string GetDefaultTitle()
        {
            if (IsArabic)
            {
                return StatusCode switch
                {
                    400 => "طلب غير صالح",
                    401 => "غير مصرح",
                    403 => "ممنوع",
                    404 => "الصفحة غير موجودة",
                    500 => "خطأ داخلي في الخادم",
                    503 => "الخدمة غير متاحة",
                    _ => "حدث خطأ"
                };
            }
            return StatusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Page Not Found",
                500 => "Internal Server Error",
                503 => "Service Unavailable",
                _ => "An Error Occurred"
            };
        }

        private string GetDefaultMessage()
        {
            if (IsArabic)
            {
                return StatusCode switch
                {
                    400 => "تعذر فهم الطلب.",
                    401 => "غير مصرح لك بالوصول.",
                    403 => "ليس لديك صلاحية الوصول.",
                    404 => "الصفحة المطلوبة غير موجودة.",
                    500 => "حدث خطأ داخلي. حاول لاحقا.",
                    503 => "الخدمة متوقفة مؤقتا. حاول لاحقا.",
                    _ => "حدث خطأ غير متوقع. حاول لاحقا."
                };
            }
            return StatusCode switch
            {
                400 => "The request could not be understood by the server.",
                401 => "You are not authorized to access this resource.",
                403 => "You don't have permission to access this resource.",
                404 => "The page you are looking for could not be found.",
                500 => "An internal server error occurred. Please try again later.",
                503 => "The service is temporarily unavailable. Please try again later.",
                _ => "An unexpected error occurred. Please try again later."
            };
        }
        
        // Method to add additional context data
        public void AddData(string key, string value)
        {
            AdditionalData[key] = value;
        }
        
        // Method to get user-friendly error description
        public string GetUserFriendlyDescription()
        {
            if (IsArabic)
            {
                return StatusCode switch
                {
                    404 => "ربما نُقلت الصفحة أو حُذفت أو أدخلت رابطا خاطئا.",
                    500 => "حدث خطأ من جهتنا. أُخطر الفريق ويعمل على إصلاحه.",
                    503 => "نجري صيانة حاليا. يرجى العودة بعد دقائق.",
                    _ => DisplayMessage
                };
            }
            return StatusCode switch
            {
                404 => "The page you're looking for might have been moved, deleted, or you entered the wrong URL.",
                500 => "Something went wrong on our end. Our team has been notified and is working to fix it.",
                503 => "We're currently performing maintenance. Please check back in a few minutes.",
                _ => DisplayMessage
            };
        }
    }
}

