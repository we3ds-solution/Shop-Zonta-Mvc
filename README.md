# Zonta Egypt — Online Store (ASP.NET Core MVC)

Egyptian streetwear storefront for **Zonta / زونتا** — premium Arabic sneakers
(Hob, Shaghaf, Fakhr), apparel, and accessories. Transformed from a generic demo
marketplace into a production-oriented store with secured checkout.

> Data honesty: `https://shopzonta.com` was inaccessible on 2026-10-10 (Cloudflare 409),
> so no official product prices/stock are seeded. The catalog starts with structural
> categories only; verified import is manual (see `docs/ZONTA-CATALOG.md`).
> Brand story facts come from scenestyled.com (13 Feb 2025) and LinkedIn — not prices.

## Technology stack (verified)

- ASP.NET Core MVC 9.0 / .NET 9.0.318 SDK
- Entity Framework Core 9.0.10 + SQL Server
- ASP.NET Core Identity (roles: Admin, Manager, Support, Customer)
- MediatR 14.0.0 (CQRS), AutoMapper 12.0.1, FluentValidation 11.9.0
- Razor views + Bootstrap 5 + session-backed cart (no extra infrastructure)
- xUnit tests: `tests/Zonta.Tests`

## Prerequisites

- .NET 9 SDK
- SQL Server 2019+ (or LocalDB for dev)
- Git

## Local setup

```bash
git clone <repo-url>
cd <repo>
dotnet restore
dotnet build
dotnet test
```

Configure the connection (never commit secrets):

```powershell
$env:ConnectionStrings__DefaultConnection="Server=(localdb)\mssqllocaldb;Database=zonta_store_db;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
dotnet run --project market-mvc
```

Or use user-secrets:

```bash
dotnet user-secrets init --project market-mvc
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;..." --project market-mvc
```

Copy `.env.example` for the full variable list (placeholders only).

## Database migrations

Migrations live in `market-mvc/Infrastructure/Migrations`. Startup applies pending
migrations explicitly via `MigrateAsync()` (no `EnsureCreated` in migration-managed DBs),
except in production where auto-migrate requires explicit `ZONTA_AUTO_MIGRATE=true`.

```bash
dotnet ef migrations add "DescribeChange" --project market-mvc
dotnet ef database update --project market-mvc
```

Never run migrations against production without a backup + reviewed deployment plan.
Existing data/relationships are preserved; seeders are idempotent.
Note: `AddProductVariants` also syncs a stale snapshot (Identity tables, index renames,
`Items` table drop) — review its header warning before any production apply.

## Development seed

Startup runs idempotent seeders after migrations:

- Roles always (Admin/Manager/Support/Customer).
- Zonta categories always (Sneakers, Apparel, Accessories, Collections); legacy demo
  categories are deactivated. No products are auto-seeded (see catalog docs).
- No demo customers/orders. Dev-only demo customer (random password) in Development only.
- Bootstrap admin only with explicit env opt-in (below).

## First administrator provisioning

No default passwords exist. Provision explicitly:

```powershell
$env:ZONTA_SEED_ADMIN="true"
$env:ZONTA_ADMIN_EMAIL="admin@example.com"
$env:ZONTA_ADMIN_PASSWORD="<strong-unique-password>"
dotnet run --project market-mvc
```

Idempotent (skips if email exists). Password is never logged. Remove/rotate the
bootstrap credentials immediately after first login. Never commit them.

## Build and test

```bash
dotnet --info
dotnet restore
dotnet build market-mvc.sln --no-restore
dotnet test market-mvc.sln --no-build
```

Tests cover: server-side order totals, cart totals, stock validation, EGP default,
order-item snapshots, variant labels/pricing/availability, EN/AR localization switching
(including validation messages), TOTP vectors, order transitions, controller ownership,
full HTTP flows (register/login/checkout/ownership/authorization) via WebApplicationFactory
+ SQLite, Paymob provider with mocked gateway, rate-limit keys, email senders, shipping rules.
CI (`.github/workflows/build-test.yml`) runs secret-scan (Gitleaks CLI) + build + test.
Note: on locked-down Windows paths (e.g. Downloads) WDAC may block the test runner;
copy the repo to an approved path and run `dotnet test` there (verified 14/14).

## Store workflows

- Home `/` — hero, featured/on-sale/latest (real DB data), brand story, empty states.
- Shop `/Products` — search, EGP price-range filter, sort, pagination.
- Detail `/Products/Details/{id}` — server price, stock state, add-to-cart (POST+antiforgery).
- Cart `/Cart` — session cart, server-price recalculation, quantity validation.
- Checkout `/Cart/Checkout` (auth required) — contact/shipping validation, cash-on-delivery
  only (Pending; no online payment pretended, no card collection), transactional order
  creation with stock decrement, PRG + cart clear against duplicates.
- Confirmation `/Cart/Confirmation/{id}` — owner/privileged only.
- Orders `/Orders` — customers see only their own; Admin/Manager/Support see all;
  status updates use valid transitions only (no total/payment overposting).
- Account `/Auth/Profile`, orders history, login/register/logout, access denied.
- Contact / Privacy / Shipping / Returns — bilingual operational policies (last updated 2026-10-11)
  based on verified store rules (COD, free shipping ≥ 750 EGP, Egypt-only).

## Production deployment checklist

- [ ] Rotate the previously committed database password (compromised) + any reused creds.
- [ ] Set `ConnectionStrings__DefaultConnection` via hosting secret manager (no secrets in repo).
- [ ] Set `ZONTA_SEED_ADMIN`/`ZONTA_ADMIN_EMAIL`/`ZONTA_ADMIN_PASSWORD` only for first boot, then unset.
- [ ] Apply migrations with backup + review (`dotnet ef database update` in release pipeline,
      or first boot with `ZONTA_AUTO_MIGRATE=true` after review — never silently).
- [ ] Enforce HTTPS, HSTS; cookies are `Secure=Always` in production.
- [ ] Confirm `Zonta:Currency` (default EGP) + `Zonta:ShippingFee` (default 0) with business.
- [ ] Import verified catalog from shopzonta.com (`docs/ZONTA-CATALOG.md`, `import-catalog` command),
      then set stock counts and activate products/variants only after confirming availability.
- [ ] Configure production SMTP (`Smtp:Host/Port/User/Pass/From`) and flip on email-confirmation
  enforcement; until then confirmation/reset emails use the dev file sender and login stays open.
- [ ] Configure payment/shipping providers if COD is insufficient (currently COD only, behind `IPaymentProvider`).
- [ ] Point domain + verify SEO/canonical URLs.
- [ ] Optional: purge git history after rotation — `docs/HISTORY-PURGE.md`.

## Deploying to MonsterASP (runasp.net)

The app ships with an IIS-ready `market-mvc/web.config` (InProcess, `AspNetCoreModuleV2`) and
`Properties/PublishProfiles/MonsterAsp.pubxml`.

1. **Publish** from Visual Studio: `Build → Publish` using the publish profile you downloaded
   from the MonsterASP Control Panel (Web Deploy), or `dotnet publish market-mvc/market-mvc.csproj -c Release`
   and upload the output folder via the Control Panel's File Manager / FTP.
2. **Website settings** (Control Panel): runtime `.NET 9`, hosting model `InProcess` (recommended),
   32-bit default is fine.
3. **Database**: create an MSSQL database in the Control Panel. Its hostname resolves to a
   private IP (`10.x`) — reachable only from MonsterASP web servers, not from your PC.
   Copy the connection string and set it as an **environment variable** on the website
   (Control Panel → environment variables, applied on next start):

   | Name | Value |
   |------|-------|
   | `ConnectionStrings__DefaultConnection` | `Server=dbXXXXX.databaseasp.net;Database=dbXXXXX;User Id=dbXXXXX;Password=...;Encrypt=False;MultipleActiveResultSets=True` |

   Never put the connection string in `appsettings.Production.json` in the repo (secrets stay out of git).
4. **First start — schema is created automatically**: the app detects a fresh/empty database,
   applies all migrations and seeds roles + the 4 brand categories itself
   (no `ZONTA_AUTO_MIGRATE` needed for an empty database; that flag is only required to migrate an
   *existing* production database, after backup + review).
5. **First admin** (optional, first boot only): also set `ZONTA_SEED_ADMIN=true`,
   `ZONTA_ADMIN_EMAIL=...`, `ZONTA_ADMIN_PASSWORD=<strong>`, then unset them after login works.
6. **HTTPS**: Control Panel → SSL → issue the free Let's Encrypt certificate; the app enforces
   HTTPS + HSTS in production.
7. **If the site returns HTTP 500.30** (app failed to start): open the Control Panel File Manager
   and read `logs/stdout_*.log` — the exact startup exception is written there
   (`web.config` has `stdoutLogEnabled="true"` for this purpose; flip it to `false` once the site runs).
8. **Swagger** (optional): set `Swagger__Enabled=true` to expose `/swagger` on the server;
   it is off in production by default.
9. **SMTP**: set `Smtp__Host/Port/User/Pass/From` to enable real confirmation/reset emails
   (issue #4) — until then login works without email confirmation.

Free plan note: 256 MB dedicated RAM. The stack fits, but upgrade if the app pool recycles under load.

## Security notes

- Remediated: live DB connection string removed from `appsettings.json` (treat as compromised, rotate now).
- Remediated: hardcoded admin/demo passwords removed; env-gated bootstrap; demo accounts dev-only.
- Remediated: cookie paths fixed (`/Auth/*`), `Secure` production-only strict, `SameSite=Lax`, HttpOnly.
- Remediated: order creation no longer trusts client totals; server prices/stock/transaction; COD pending (never paid from client).
- Remediated: order listing/details enforce ownership (no IDOR enumeration); admin edits are status-only.
- Remediated: `EnsureCreatedAsync` replaced with explicit `MigrateAsync`; startup fails fast on DB errors.
- Auth hardening: email confirmation + password reset flows implemented (`IEmailSender`;
  SMTP when `Smtp:*` configured, dev file-sender otherwise; enforcement stays off until
  production SMTP exists so registration never locks out). Login/register/checkout POSTs
  rate-limited (5/min/IP, HTTP 429). Account lockout (5 attempts/15 min) unchanged. TOTP 2FA available.
- CI: Gitleaks CLI secret scan on every push/PR (working tree).
- Remaining risks: git history may still contain old secrets until a maintainer runs the purge in
  `docs/HISTORY-PURGE.md` (issue #13); uploads need AV/size review for large catalogs.

## Known limitations

- Catalog holds 14 products verified from the official homepage archive (2024-12-21);
  per-product descriptions, materials, sizes, and stock counts were never archived — absent
  fields mean unknown, never guessed. Import more via `import-catalog`.
- Imported products show "Available" without quantity claims (stock counts are never
  invented); variants start inactive/zero-stock for admin confirmation.
- EN/AR UI switching is implemented (real cookie-driven culture + RTL + localized nav/hero/
  currency, product names never translated); deeper view-by-view translation is follow-up.
- No online payment gateway configured — COD default behind `IPaymentProvider`;
  a Paymob provider is implemented and mock-tested, awaiting live API keys.
- No authorized shopzonta.com sync API — manual import only (no fragile scraper).

## Product data import

See `docs/ZONTA-CATALOG.md` + `market-mvc/Infrastructure/Data/Catalog/ZontaCatalog.json`.
Record source URL + date per product; match by SKU; never duplicate or overwrite manual inventory blindly.
New variants import as inactive/zero-stock for admin confirmation.

## Brand imagery (local assets)

Real brand photos scraped from the official Instagram (@shop.zonta) and stored locally — no fabricated art,
no hotlinked social URLs at runtime:

- `market-mvc/wwwroot/images/social/` — 11 product posts + profile picture (`ig-{shortcode}.jpg`)
  with `ig-map.json` (shortcode, caption, alt text, source URL, KB size).
- `market-mvc/wwwroot/images/categories/{footwear,accessories,collections,apparel}/` — same photos
  mapped to the four seeded categories (Habibi/Fakhr/Shaghaf themes).
- `market-mvc/wwwroot/images/brand/profile.jpg` — brand avatar.
- `market-mvc/wwwroot/images/social/category-mapping.json` — full caption/alt/source mapping.

Wired into views (all `loading="lazy"`, localized EN/AR alt text via `Gallery.*` / `Card.BrandAlt` keys):

| Surface | Fallback behavior |
|---|---|
| `_ProductCard` / `_ProductList` / `Cart` / `Products/Details` | Brand photo instead of grey placeholder icon |
| Home + Products empty states | 4-up brand gallery (keeps SEO + brand feel while catalog is empty) |
| Footer | 6-image brand strip on every page |
| About + Contact | Verified hero/collection banner |
| `_Layout` `<head>` | `og:image` + `twitter:image` for social sharing |

Refresh: re-run the Instagram extraction (codes in `ig-map.json`), replace files in-place, keep same names.
