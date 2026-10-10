# Complete Repository Refactoring & Transformation — Zonta Egypt E-commerce

## 1. Role and Mission

Act as a senior .NET architect, ASP.NET Core MVC engineer, e-commerce domain specialist, database engineer, security engineer, and UI/UX designer.

Repository:
https://github.com/we3ds-solution/Shop-Zonta-Mvc

Official brand and research sources:
- Official store: https://shopzonta.com
- Official Instagram: https://www.instagram.com/shop.zonta/?hl=en
- Brand-related Facebook video: https://www.facebook.com/ebank.egypt/videos/zonta/3089042797912901/
- Brand editorial reference: https://scenestyled.com/Styled/Egyptian-Streetwear-Label-Zonta-Debuts-First-Ever-Arabic-Sneakers

Company: WE3DS  
GitHub organization: we3ds-solution  
Preferred commit author email: we3ds.solution@gmail.com

Your mission is to inspect, clean, refactor, secure, and transform the existing legacy application into a polished, production-oriented Zonta Egypt online store.

This is an implementation task, not merely a review or a proposal. Inspect the repository, establish its current state, create a prioritized plan, implement the required changes, run the available checks, and document everything that remains incomplete.

Do not stop after generating a report. Continue with the implementation unless blocked by missing access, unavailable dependencies, missing credentials, or a decision that genuinely requires human approval.

## 2. Mandatory Rules

1. Inspect the actual repository, solution files, project files, source code, Git history where useful, configuration, migrations, views, assets, tests, and documentation before making architectural decisions.
2. Identify the actual .NET version, ASP.NET Core MVC version, database provider, Entity Framework Core version, authentication implementation, and current deployment assumptions. Do not assume the project uses a particular version.
3. Preserve working business functionality and useful architecture. Do not rewrite everything from scratch without evidence that a rewrite is necessary.
4. Remove obsolete demo-store branding, mock products, unrelated categories, fake statistics, unused code, dead assets, duplicate implementations, and stale documentation.
5. Do not delete working features or database structures before identifying their dependencies and migration requirements.
6. Never fabricate Zonta product information, product availability, prices, sizes, colors, images, policies, testimonials, reviews, or company claims.
7. Use the official website as the primary source for actual product information. Use Instagram and the other supplied sources only to support brand identity and facts that can be verified.
8. If an official source cannot be accessed, document the limitation and keep the affected information configurable or mark it as requiring verification.
9. Never expose, repeat, log, commit, or place secrets in source code, reports, screenshots, test fixtures, or documentation.
10. Never commit `.env` files, passwords, database connection strings containing credentials, private API keys, access tokens, or production customer data.
11. Do not run destructive production database operations, reset a database, drop tables, delete production data, or overwrite production configuration.
12. Do not make up test results. Run the available commands and report the real outcome.
13. Do not claim the application is production-ready until the relevant build, security, authorization, database, and functional checks have passed.
14. Keep the code maintainable, strongly typed, testable, accessible, responsive, and consistent with the existing technology stack.
15. Explain your final results in Arabic, including changed files, security issues, tests, remaining manual actions, and deployment requirements.

## 3. Phase One — Repository Audit

Before editing, inspect:

- All `.sln`, `.csproj`, project references, package versions, and build configuration.
- `Program.cs`, configuration files, dependency injection registrations, middleware, routing, and environment-specific settings.
- Controllers, services, repositories, interfaces, CQRS/MediatR handlers, validators, view models, domain entities, and mapping configuration.
- `AppDbContext`, EF Core entity configurations, relationships, migrations, database initialization, and all seeders.
- ASP.NET Core Identity, roles, authentication cookies, authorization attributes, policies, registration, login, logout, and account recovery.
- Product, category, customer, cart, order, order item, payment, shipping, and inventory implementations.
- All Razor views, layouts, partials, static assets, CSS, JavaScript, fonts, icons, and image references.
- Forms, validation, filtering, pagination, sorting, and user-facing error handling.
- Tests, GitHub Actions, dependency security configuration, README, `.gitignore`, and deployment instructions.

Create an internal inventory of:
- Correct and reusable components.
- Obsolete components.
- Broken references and namespace inconsistencies.
- Duplicate business logic and unused dependencies.
- Security vulnerabilities.
- Database migration and compatibility risks.
- Missing features required by a real apparel/streetwear e-commerce store.

Pay special attention to stale namespaces such as `newApp.Models...` if they conflict with the current `market_mvc` namespaces.

Use the actual repository structure rather than blindly following example paths from previous reviews.

## 4. Phase Two — Security Remediation

Security fixes take priority over visual redesign.

### Secrets and configuration
- Inspect tracked configuration and relevant Git history for exposed credentials.
- If credentials have been committed, treat them as compromised and recommend immediate rotation.
- Remove secrets from the current code and configuration.
- Configure secret handling using environment variables, .NET user secrets for development, or the hosting platform's secret manager.
- Provide a safe configuration template containing placeholders only.
- Ensure secret values are never logged or printed in reports.
- Check `.gitignore` for environment files, local configuration, build artifacts, IDE files, logs, and generated secrets.
- If history rewriting is necessary, document a safe procedure and obtain approval before rewriting shared history.

### Identity and authorization
- Remove hardcoded administrator, manager, support, and demo-user passwords from production startup logic.
- Do not create privileged accounts with predictable passwords.
- Provide a safe first-admin provisioning process using environment-provided bootstrap credentials or a controlled administrative setup process.
- Do not log bootstrap credentials.
- Make bootstrap provisioning idempotent and restrict it to an explicitly authorized environment or operation.
- Verify role assignment failures and handle them correctly.
- Review password policy, account lockout, cookie settings, antiforgery protection, session behavior, and logout semantics.
- Configure secure cookies appropriately for HTTPS deployments without making local development unusable.
- Require authorization on all administrative operations.
- Ensure customers can access only their own private orders and account information.
- Do not trust a submitted customer ID, price, discount, order total, payment status, or role from the client.
- Verify that login, logout, access-denied, and account recovery routes actually exist and work.

### Order and payment security
- Calculate product prices and totals on the server using authoritative database values.
- Validate quantities, stock, product status, selected variants, discounts, and shipping fees.
- Prevent negative quantities, invalid prices, duplicate processing, and inconsistent order totals.
- Never mark an order as paid based solely on a client-submitted status.
- If no payment provider is configured, implement an honest payment-pending or supported cash-on-delivery workflow instead of pretending online payment works.
- Do not collect or store raw card details.
- Use database transactions where necessary to keep orders, order items, and inventory updates consistent.
- Handle concurrency and repeated submissions safely.

### Input and application security
- Validate and normalize user input.
- Use antiforgery protection for state-changing browser forms.
- Prevent overposting by using dedicated input view models.
- Validate uploaded files if image uploads exist.
- Review XSS, open redirects, insecure direct object references, SQL injection risks, unsafe file paths, and excessive error disclosure.
- Use parameterized database operations and appropriate output encoding.
- Configure production error handling and HTTPS redirection appropriately.
- Avoid exposing stack traces or internal configuration to users.

## 5. Phase Three — Database and Architecture Cleanup

### Entity Framework Core
- Inspect how migrations are currently managed.
- Do not rely on `EnsureCreatedAsync()` as a substitute for migrations in an existing migration-managed production database.
- Replace unsafe automatic schema creation with an explicit, environment-appropriate migration strategy.
- Do not run migrations against a production database without authorization, a backup, and a reviewed deployment plan.
- Preserve existing data and relationships.
- Add migrations only when the entity/schema changes justify them.
- Verify foreign keys, indexes, delete behavior, required fields, decimal precision, date/time handling, and uniqueness constraints.
- Make database initialization deterministic, observable, and safe to retry.
- Do not swallow initialization exceptions and then report successful startup.
- Keep development/demo seeding separate from production provisioning.
- Make seeders idempotent and ensure their dependencies and execution order are correct.

### Domain model
Review and refactor the existing models to support the actual store.

Where justified by the existing implementation, ensure support for:
- Products and categories.
- Product images and image ordering.
- Product variants, including size and color where applicable.
- SKU and inventory.
- Regular and sale prices when verified and required.
- Product publication and availability.
- Shopping cart items.
- Customer information.
- Orders and immutable order-item price/name snapshots.
- Shipping addresses and order status history where appropriate.
- Discounts or coupons only if the business rules are known and implemented correctly.

Avoid speculative complexity. Do not introduce microservices, event buses, distributed caches, or new infrastructure without a demonstrated need.

If the current product model cannot accurately represent apparel variants, introduce the smallest maintainable schema change that supports them.

Never invent stock levels or imply an unavailable product is in stock.

### Architecture
- Resolve namespace and type mismatches.
- Eliminate duplicated business rules.
- Keep controllers focused on HTTP concerns.
- Move business logic into appropriate application/domain services or existing CQRS handlers.
- Use dependency injection consistently.
- Use asynchronous I/O and cancellation tokens where appropriate.
- Use DTOs/view models rather than exposing persistence entities unnecessarily.
- Avoid redundant repository abstractions if they add no value over the current architecture.
- Remove unused dependencies only after checking their usage.
- Keep the code style and naming conventions consistent.

## 6. Phase Four — Official Zonta Data Collection

Research the supplied sources and the official store's publicly accessible pages.

Build a verified catalog containing, when actually available:
- Product name.
- Product URL.
- Product description.
- Current displayed price and currency.
- Sale price only when officially displayed.
- Available sizes.
- Available colors.
- Stock/availability state.
- SKU or product identifier if publicly available.
- Product image URLs.
- Image alt text.
- Product category.
- Relevant product details and materials if officially stated.
- Evidence/source URL and date checked.

Inspect product listing pages, product detail pages, and relevant collections.

Do not treat editorial mentions or social media posts as authoritative sources for current prices or stock if the official product page is available.

### Data integrity requirements
- Do not invent missing products, variants, prices, descriptions, inventory, or policies.
- Do not treat every image found online as a product image.
- Avoid duplicates when a product appears in several collections.
- Do not download or redistribute third-party assets unless their usage is permitted.
- Prefer official image URLs or approved locally stored assets.
- If a source is inaccessible, add the field to a missing-data report rather than guessing.
- Never silently substitute unrelated demo products.

Create a clear data import/update strategy appropriate to the repository:
- Prefer structured seed/catalog data for a small fixed catalog.
- Use an import command or service if recurring catalog updates justify it.
- Keep product data separate from UI markup.
- Ensure re-running the import does not duplicate products.
- Do not overwrite manually maintained inventory or order data.
- Document which data fields are sourced from the official store and which require manual confirmation.

Use EGP only when the official source confirms the price is in EGP. Format currency correctly for the chosen language and locale.

If there is no authorized API or feed, do not assume the external website can be synchronized automatically. Implement a maintainable manual import/update workflow instead of building a fragile scraper.

## 7. Phase Five — Complete Brand and UI Transformation

Remove old generic marketplace/demo branding and create a consistent Zonta Egypt shopping experience.

### Branding
- Use the official brand name and verified brand assets.
- Inspect the official store and public brand references for typography, color palette, photography, logo treatment, tone, and visual direction.
- Do not copy an unrelated store's design.
- Do not fabricate brand slogans or brand claims.
- Replace old names such as `Market MVC`, `newApp`, and unrelated sample-store references wherever appropriate.
- Update page titles, metadata, navigation, footer, email templates if present, and user-facing messages.

### Required customer-facing pages
Implement or repair the pages appropriate to the existing MVC architecture:

1. Home page.
2. Product listing/catalog.
3. Product detail.
4. Search results.
5. Category/collection pages where supported.
6. Shopping cart.
7. Checkout and order confirmation.
8. Login and registration.
9. Customer account/profile.
10. Customer order history and order details.
11. Contact/support page using only verified contact information.
12. Privacy policy, terms, shipping, returns, and refund pages only using approved, accurate content.
13. Not-found and error pages.

Do not publish placeholder legal policies as if they were approved company policies.

### Home page
Replace the old dashboard-oriented landing page with a real storefront:
- Strong brand-led hero section.
- Clear primary shopping action.
- Featured products backed by verified data.
- Product collections or categories backed by real catalog data.
- Product cards with consistent imagery, titles, and verified prices.
- A coherent brand story section only when supported by reliable information.
- Responsive navigation and footer.
- Useful empty states when the catalog is unavailable.

Do not show administrative statistics such as total customers or total orders on the public storefront.

### Product listing and detail
- Display real product data from the application/database.
- Add working category filtering, search, sorting, and pagination as appropriate.
- Use stable product URLs and clear navigation.
- Provide responsive product cards and consistent image ratios.
- Show variant selectors only for real variants.
- Validate selected size/color and stock on the server.
- Disable purchase actions when a product is unavailable.
- Make loading, empty, invalid-selection, and error states clear.
- Use accessible labels and keyboard navigation.

### Shopping cart and checkout
- Implement a complete cart flow consistent with the existing authentication and persistence model.
- Show accurate item subtotals and order totals.
- Validate stock and prices again during checkout.
- Preserve cart contents appropriately across navigation and authentication.
- Prevent duplicate order submission.
- Validate contact and shipping information.
- Show a clear confirmation page only after successful order creation.
- Ensure the customer can view their own order and no one else's.

### Administrative experience
Preserve and improve the admin area if it is useful:
- Product/category management.
- Product image and variant management if supported.
- Inventory management.
- Order listing and order details.
- Order status updates with authorization and valid transitions.
- Customer support tools only where justified.
- Search, filtering, pagination, and meaningful validation.
- No public access to administrative actions.

Keep storefront and administration clearly separated through routing, layouts, navigation, and authorization.

### Responsive design and accessibility
- Support mobile, tablet, and desktop.
- Avoid horizontal overflow and broken layouts.
- Use semantic HTML, accessible form labels, keyboard-friendly interactions, and visible focus states.
- Provide meaningful image alt text.
- Maintain readable contrast.
- Respect reduced-motion preferences.
- Use responsive images, lazy loading for below-the-fold images, and appropriate dimensions.
- Avoid adding heavy JavaScript libraries unless needed.

### SEO and performance
- Add meaningful page titles and meta descriptions.
- Add canonical URLs only when the correct public URL is known.
- Add Open Graph metadata.
- Implement product structured data only when it accurately reflects visible, verified data.
- Use pagination URLs and query parameters consistently.
- Avoid duplicate metadata and broken links.
- Optimize image loading and static assets.
- Use production caching only where safe.
- Do not cache customer-specific account, cart, checkout, or private order data publicly.

## 8. Phase Six — Localization and Currency

Inspect the official store to determine its actual language and regional behavior.

- Support Arabic RTL and English LTR if appropriate for the target audience.
- Ensure layout direction, navigation, forms, icons, and product cards render correctly in both directions.
- Keep user-facing text in a consistent localization system rather than scattering translations through views.
- Do not automatically translate product names or official descriptions if that changes the brand's intended wording.
- Use culture-aware formatting for prices and dates.
- Use the verified official currency; do not assume a price's currency from its numeric value.
- Ensure validation messages and checkout errors are localized.

Do not add a language selector that does not actually switch content.

## 9. Phase Seven — Remove Old and Unused Material Safely

Search the repository for old branding, demo product names, unrelated categories, obsolete URLs, old namespaces, placeholder images, fake statistics, test credentials, and unused files.

Examples to investigate include:
- `Market MVC`
- `newApp`
- `market-mvc.local`
- Sample electronics, books, household products, sports products, and unrelated demo catalog content.
- Hardcoded demo prices and fake inventory.
- Stale repository URLs.
- Outdated README instructions.
- Dead CSS, JavaScript, images, and unused packages.

For every removal:
1. Search for references.
2. Check whether it is used by routing, DI, Razor views, JavaScript, tests, migrations, or runtime configuration.
3. Remove or refactor the dependency safely.
4. Update related tests and documentation.
5. Run the build and tests.

Do not delete a file solely because its name appears old. Confirm it is obsolete and not needed by the application.

Preserve Git history. Do not delete production data or existing customer/order records.

## 10. Phase Eight — Testing and Verification

Run the commands supported by the actual repository and SDK. For example, if the project supports them:

- `dotnet --info`
- `dotnet restore`
- `dotnet build --no-restore`
- `dotnet test --no-build`

Add or update tests for critical business logic.

At minimum, verify:
- The solution builds without errors.
- Controllers and views compile.
- DI registrations resolve.
- Database migrations and initialization are consistent.
- Product catalog queries, filters, and pagination work.
- Product variants and stock validation work if implemented.
- Cart calculations are correct.
- Order totals are calculated on the server.
- Duplicate submissions do not create duplicate orders.
- Customers cannot access other customers' orders.
- Unauthorized users cannot perform administrative actions.
- Invalid input is rejected.
- Seeders can run repeatedly without duplicating records.
- No production secrets or hardcoded demo credentials remain in tracked current files.
- Navigation, forms, routes, and asset references are valid.

Use a test database or isolated environment. Never use production customer data for testing.

If browser testing tools are available, run responsive smoke tests on the home page, catalog, product detail, cart, checkout, and authentication flows.

If any test cannot run because credentials, infrastructure, or tools are unavailable, clearly mark it as NOT RUN and state why. Do not claim success without evidence.

## 11. Phase Nine — Documentation and Deployment

Update the README to include:
- Zonta Egypt project overview.
- Actual technology stack and versions.
- Prerequisites.
- Local setup instructions.
- Safe configuration examples.
- Database migration instructions.
- Safe development seed instructions.
- First administrator provisioning procedure.
- Build and test commands.
- Production deployment checklist.
- Security notes.
- Known limitations.
- How to import or update verified product data.

Add or update:
- `.gitignore`
- `.env.example` only if the project actually uses environment files, with placeholders and no secrets
- CI build/test workflow where practical
- Dependency/security checks supported by the repository
- A concise architecture note if it materially helps maintenance

Do not include credentials or private customer data in documentation.

## 12. Phase Ten — Git and Commit Rules

Use the repository's existing branch and collaboration rules.

Preferred commit author email:
`we3ds.solution@gmail.com`

Before creating commits:
- Inspect `git status`, current branch, remotes, and existing uncommitted changes.
- Do not overwrite or discard work that was already present.
- Do not rewrite shared history.
- Do not push directly to a protected branch if the repository requires pull requests.
- Do not bypass branch protection or required checks.
- Do not use another person's credentials or modify GitHub organization permissions.
- Do not change the GitHub account email or account settings just to satisfy the commit author requirement.

If authorized and appropriate, configure the commit-local identity:

`git config user.name "WE3DS Solution"`

`git config user.email "we3ds.solution@gmail.com"`

Verify the resulting commit author. Remember that Git author metadata does not prove the email is verified by GitHub or that the commit is cryptographically signed.

Prefer a focused branch and coherent commits, for example:
- `fix: secure configuration and admin provisioning`
- `refactor: repair domain and database initialization`
- `feat: replace demo catalog with verified Zonta products`
- `feat: implement Zonta storefront and product variants`
- `fix: secure cart checkout and order authorization`
- `test: add critical commerce workflow coverage`
- `docs: document setup and deployment`

Adjust the commit structure to match the actual changes. Do not create empty commits or claim a commit/push succeeded if it did not.

If repository write access, Git identity, authentication, or permission is missing, complete all safe local work possible and provide the exact manual steps required. Never expose access tokens.

## 13. Required Final Report — Arabic

At the end, provide a structured Arabic report containing:

1. **ملخص التنفيذ:** What was actually changed.
2. **حالة البناء والاختبارات:** Commands executed and real outcomes.
3. **التغييرات حسب الملفات:** Important files created, modified, or removed, and why.
4. **الأمان:** Exposed secrets remediated, credentials rotated or still awaiting manual rotation, authentication changes, and remaining risks.
5. **بيانات Zonta:** Verified products and fields collected, their sources, and information still requiring confirmation.
6. **الواجهة:** Pages and customer workflows implemented.
7. **قاعدة البيانات:** Migrations created, schema changes, and any manual database operations required.
8. **Git:** Branch, commit hashes, author identity, push/PR status, and any blockers.
9. **الخطوات اليدوية المطلوبة:** Hosting configuration, secret rotation, database migration approval, product data confirmation, payment/shipping provider setup, domain/HTTPS, and any other outstanding requirements.
10. **المشكلات المتبقية:** Every unresolved issue and its priority.

For each feature, classify it honestly as:
- Implemented and tested.
- Implemented but not fully tested.
- Partially implemented.
- Not implemented.
- Blocked pending access or business confirmation.

## 14. Execution Order

Work in this order:

1. Inspect repository state and identify the actual application architecture.
2. Report critical security issues internally and remediate safe source-code issues first.
3. Resolve compilation, namespace, dependency injection, and configuration problems.
4. Correct database initialization and migration strategy without touching production data.
5. Fix authentication, authorization, customer privacy, and order calculation.
6. Research official Zonta data and produce a verified catalog.
7. Remove obsolete demo branding and data safely.
8. Implement the Zonta storefront, catalog, product detail, cart, checkout, and required administration.
9. Add or update tests.
10. Update documentation and CI.
11. Run final verification.
12. Commit only if authorized and technically possible.
13. Deliver the Arabic report.

Start by inspecting the actual repository now. Do not merely repeat this plan back to me. Implement the highest-priority safe changes, verify them, and continue until finished or genuinely blocked. Do not invent unavailable product data, credentials, successful tests, commits, or deployments.