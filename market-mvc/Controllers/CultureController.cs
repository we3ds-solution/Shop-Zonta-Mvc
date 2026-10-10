using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace market_mvc.Controllers
{
    public class CultureController : Controller
    {
        // POST: /Culture/Set — actually switches UI culture (cookie + redirect back).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Set(string culture, string? returnUrl = null)
        {
            culture = culture == "ar" ? "ar" : "en";
            Response.Cookies.Append("zonta_culture", culture, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = false, // read by layout direction logic server-side per request
                SameSite = SameSiteMode.Lax,
                IsEssential = true
            });
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), SameSite = SameSiteMode.Lax, IsEssential = true });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }
    }
}
