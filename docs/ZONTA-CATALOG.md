# Zonta catalog — verified data workflow

Date checked: 2026-10-10.
Primary source `https://shopzonta.com` was **inaccessible** (Cloudflare 409 on `/`, `/sitemap.xml`, `/robots.txt`, `/products.json`).
Wayback (16 Feb 2025) showed a Shopify password page. No products, prices, currency, sizes, or contact info could be verified.

## What is verified (secondary sources only)

- Brand: **Zonta / زونتا** — Egyptian-born streetwear label, "Arabic Sneakers".
- Taglines seen: "Born in the Arab world, worn around the globe.", "The Only Sneakers That Speak Your Language.", "Premium, genuine leather.", "Produced by Arab hands".
- Drop 1.0: minimalist sneakers inscribed with **Hob (love), Shaghaf (passion), Fakhr (pride)**.
- Founders: Ahmed El Hefny and Sameh Radwan (also Amzolute). Source: scenestyled.com, 13 Feb 2025.
- Shark Tank Egypt: EGP 5M for 15% (secondary editorial claim, not product data).
- Instagram: `https://www.instagram.com/shop.zonta/?hl=en` (login-walled; snippets only).
- Hosting inference only: Shopify IPs (23.227.38.65, shops.myshopify.com CNAME).

These facts back the brand story section only. They are **not** product prices, stock, or policies.

## Missing data (do NOT guess)

Product names, URLs, descriptions, prices, sale prices, sizes, colors, stock state,
SKUs, image URLs/alt, categories, materials, contact info, shipping/return policies,
currency confirmation on product pages.

## Import workflow (manual, idempotent)

1. Open each official product page on shopzonta.com.
2. For each product, record: name, URL, description (verbatim), displayed price + currency,
   sale price only if displayed, sizes, colors, availability text, SKU if shown,
   image URLs + alt, category, materials, source URL, date checked.
3. Add an entry to `market-mvc/Infrastructure/Data/Catalog/ZontaCatalog.json`:
   ```json
   {
     "sku": "ZONTA-HOB-001",
     "name": "...",
     "slug": "hob-sneaker",
     "description": "...",
     "price": 0,
     "compareAtPrice": null,
     "currency": "EGP",
     "sizes": [],
     "colors": [],
     "availability": "unknown",
     "imageUrl": null,
     "imageAlt": null,
     "category": "Sneakers",
     "sourceUrl": "https://shopzonta.com/products/...",
     "dateChecked": "2026-10-10"
   }
   ```
4. Import via the maintained command (idempotent by Sku, preserves manual stock):
   ```bash
   dotnet run --project market-mvc -- import-catalog --file market-mvc/Infrastructure/Data/Catalog/ZontaCatalog.json
   ```
   New variants are created inactive with zero stock; an admin must confirm
   availability (Products → Edit → Variants) before sale. Or use the admin
   product create page (validates price/stock/status server-side).
5. Re-running must match by `Sku` and update — never duplicate. Never overwrite manual inventory counts blindly.
6. Keep product data separate from UI markup. Official image URLs preferred; do not
   download/redistribute third-party assets unless permitted.

## Currency

`Zonta:Currency` defaults to `EGP` based on Egyptian context but **requires confirmation**
on official product pages. Do not assume currency from numeric values.
