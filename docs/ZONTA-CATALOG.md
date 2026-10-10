# Zonta catalog — verified data (official store)

## Verified source

Official `https://shopzonta.com` homepage, Wayback capture **2024-12-21**
(`https://web.archive.org/web/20241221051009/https://shopzonta.com/`).
Live site returns Cloudflare 409; sub-pages were not archived. Checked 2026-10-10
(initial) and re-checked via archive.

### Verified brand facts (official HTML)

- `<html lang="ar">`, Arabic-first RTL; English version published at `/en`.
- Title: `زونتا | أحذية متينة من الألياف الدقيقة` (Zonta | durable microfiber shoes).
- Tagline: `معاك في اي حاجة`. Hero: `معاك في كل خطوة، وفي كل مناسبة`.
- `og:site_name`: `Zonta`. Favicon: Arabic Zonta mark (`cdn/shop/files/uppdated_zonta_favicon_-_arabic.png`).
- Fonts: **Somar Sans** (headings 800, body 400).
- Colors: bg #FFFFFF, text #222222, muted #666666, borders #EEEEEE, **buttons BLACK/white
  UPPERCASE 5px**, sale #E84E4E, footer #F5F5F5, badges hot #9A84C8 / new #98D8CA / sale #DA3F3F.
- **Currency: EGP** (`Shopify.currency.active=EGP`, `money_format={{amount}} EGP`).
- Sections: `الأفضل مبيعاً` (best sellers), `حريمي` (women), `رجالي` (men).
- Collections: `all`, `all-shoes`, `mens-collections`, `womens-collection`.
- Pages: `about`, `contact`, `exchange-and-refund`; policies: `privacy-policy`, `terms-of-service`.

### Verified products (14)

All men's/Urban Edge at **850.00 EGP**, women's ZigZag high-tops at **950.00 EGP**,
all with official CDN image URLs. See `ZontaCatalog.json` (each item has `sourceUrl`
+ `dateChecked: 2024-12-21`). No compare-at (sale) prices were displayed.
Handles `lace-up-shoes`, `slip-on-shoes-low-cut`, `tricot-slip-on-shoes` appeared
without prices and are **excluded** (never guess).

### NOT verified (do NOT guess)

Per-product descriptions, materials beyond the microfiber title, sizes, size guide,
stock counts, SKUs, ratings, contact details, shipping/return/legal texts
(sub-page captures returned 404).

## Import workflow (manual, idempotent)

1. Verify any new product on an official/archived product page.
2. Add an entry to `market-mvc/Infrastructure/Data/Catalog/ZontaCatalog.json`
   (`sku`, `name` verbatim, `price`, `imageUrl`, `category`, `sourceUrl`, `dateChecked`).
   `status: Active` only when the product is shown for sale officially;
   `trackQuantity: false` when stock counts were never published (shows
   "Available" with no quantity claims — never invent numbers).
3. Run (idempotent by Sku, preserves manual stock):
   ```bash
   dotnet run --project market-mvc -- import-catalog --file market-mvc/Infrastructure/Data/Catalog/ZontaCatalog.json
   ```
4. New variants import inactive/zero-stock for admin confirmation.
5. Official image URLs are hotlinked (no redistribution). The official CDN blocks some
   networks (Cloudflare 409) and the files were never archived, so local download is
   currently impossible without permission/access. Views therefore include an
   `onerror` fallback to a neutral local brand placeholder
   (`wwwroot/images/products/placeholder.svg`) — no fake product photos. If CDN access
   is granted, download with permission and host locally, updating `imageUrl`.

## Currency

`Zonta:Currency` = `EGP` — confirmed by official config. `Zonta:PublicUrl`
(`https://shopzonta.com`) drives canonical/OG links.
