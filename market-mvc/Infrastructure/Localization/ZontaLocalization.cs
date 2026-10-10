namespace market_mvc.Infrastructure.Localization
{
    /// <summary>
    /// Minimal consistent localization system (EN LTR / AR RTL).
    /// Product names and official descriptions are NEVER translated (brand wording preserved);
    /// only UI chrome is localized. Culture-aware price/date formatting comes from
    /// RequestLocalization (en / ar). Extend dictionaries here, not scattered view strings.
    /// </summary>
    public interface IZontaLocalizer
    {
        string Culture { get; }
        bool IsRtl { get; }
        string Get(string key);
        string CurrencyLabel { get; }
        string FormatPrice(decimal amount);
    }

    public class ZontaLocalizer : IZontaLocalizer
    {
        private readonly IHttpContextAccessor _http;

        private static readonly Dictionary<string, (string En, string Ar)> Strings = new()
        {
            ["Nav.Home"] = ("Home", "الرئيسية"),
            ["Nav.Shop"] = ("Shop", "تسوّق"),
            ["Nav.Cart"] = ("Cart", "السلة"),
            ["Nav.Orders"] = ("Orders", "الطلبات"),
            ["Nav.Contact"] = ("Contact", "اتصل بنا"),
            ["Nav.Login"] = ("Login", "تسجيل الدخول"),
            ["Nav.Register"] = ("Register", "حساب جديد"),
            ["Nav.Logout"] = ("Logout", "تسجيل الخروج"),
            ["Hero.Kicker"] = ("Egyptian streetwear · Drop 1.0", "أحذية متينة من الألياف الدقيقة"),
            ["Hero.Title"] = ("With you every step, every occasion.", "معاك في كل خطوة، وفي كل مناسبة"),
            ["Hero.Sub"] = ("Premium, genuine leather Arabic sneakers — Hob (love), Shaghaf (passion), Fakhr (pride). Produced by Arab hands, in Arab lands.", "معاك في اي حاجة — كوتشي زونتا الأصلي."),
            ["Hero.Shop"] = ("Shop the collection", "تسوّق المجموعة"),
            ["Hero.Contact"] = ("Contact us", "اتصل بنا"),
            ["Topbar.Tag"] = ("With you in everything.", "معاك في اي حاجة"),
            ["Topbar.Shipping"] = ("Free shipping on orders over 750 EGP.", "شحن مجاني لجميع الطلبات فوق 750 جنيه"),
            ["Search.Placeholder"] = ("Search sneakers, apparel...", "ابحث عن سنيكرز، ملابس..."),
            ["Footer.Brand"] = ("With you on every errand. With you everywhere.", "معاك في كل مشاوريك. معاك في كل مكان."),
            ["Cart.FreeShipNote"] = ("Free shipping on orders over 750 EGP.", "الشحن مجاني للطلبات فوق 750 جنيه."),
            ["Footer.Tag"] = ("Egyptian streetwear label. Premium, genuine leather Arabic sneakers.", "علامة ستريت وير مصرية. سنيكرز عربي فاخر من الجلد الطبيعي."),
            ["Common.BackHome"] = ("Back to home", "عودة للرئيسية"),
            ["Common.ShopNow"] = ("Shop now", "تسوّق الآن"),
            ["Common.Cod"] = ("Cash on delivery", "الدفع عند الاستلام"),
            ["Cart.Title"] = ("Your Cart", "سلة التسوق"),
            ["Cart.Empty"] = ("Your cart is empty", "سلتك فارغة"),
            ["Cart.EmptyHint"] = ("Browse the collection and add your favorites.", "تصفح المجموعة وأضف ما يعجبك."),
            ["Cart.Checkout"] = ("Checkout", "إتمام الشراء"),
            ["Cart.Continue"] = ("Continue shopping", "مواصلة التسوق"),
            ["Cart.Update"] = ("Update", "تحديث"),
            ["Cart.Remove"] = ("Remove", "إزالة"),
            ["Cart.Subtotal"] = ("Subtotal", "المجموع الفرعي"),
            ["Cart.Shipping"] = ("Shipping", "الشحن"),
            ["Cart.Total"] = ("Total", "الإجمالي"),
            ["Cart.Recalc"] = ("Totals are recalculated on the server at checkout.", "تُحسب الإجماليات على الخادم عند الدفع."),
            ["Checkout.Title"] = ("Checkout", "إتمام الشراء"),
            ["Checkout.Contact"] = ("Contact & shipping", "بيانات التواصل والشحن"),
            ["Checkout.Place"] = ("Place order", "تأكيد الطلب"),
            ["Checkout.Back"] = ("Back to cart", "عودة للسلة"),
            ["Checkout.Summary"] = ("Order summary", "ملخص الطلب"),
            ["Checkout.CodNote"] = ("Cash on delivery — payment is collected on delivery. No online payment is processed and no card details are collected.", "الدفع عند الاستلام — يُحصّل المبلغ عند التوصيل. لا تُعالج مدفوعات إلكترونية ولا تُجمع بيانات بطاقات."),
            ["Checkout.ServerNote"] = ("Final total is calculated on the server from database prices and stock.", "يُحسب الإجمالي النهائي على الخادم من أسعار قاعدة البيانات والمخزون."),
            ["Orders.Title"] = ("Orders", "الطلبات"),
            ["Orders.History"] = ("Order history", "سجل الطلبات"),
            ["Profile.Title"] = ("My account", "حسابي"),
            ["Profile.Orders"] = ("My orders", "طلباتي"),
        };

        public ZontaLocalizer(IHttpContextAccessor http)
        {
            _http = http;
        }

        public string Culture
        {
            get
            {
                var c = _http.HttpContext?.Request.Cookies["zonta_culture"];
                return c == "ar" ? "ar" : "en";
            }
        }

        public bool IsRtl => Culture == "ar";

        public string Get(string key)
        {
            if (!Strings.TryGetValue(key, out var v)) return key;
            return Culture == "ar" ? v.Ar : v.En;
        }

        public string CurrencyLabel => Culture == "ar" ? "ج.م" : "EGP";

        public string FormatPrice(decimal amount)
        {
            var culture = Culture == "ar"
                ? new System.Globalization.CultureInfo("ar-EG")
                : new System.Globalization.CultureInfo("en-US");
            return $"{amount.ToString("N2", culture)} {CurrencyLabel}";
        }
    }
}
