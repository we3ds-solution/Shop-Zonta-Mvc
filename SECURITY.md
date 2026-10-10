# Security Policy

## Supported Versions

| Version | Supported |
| ------- | --------- |
| `master` / latest | :white_check_mark: |
| Older commits | :x: |

This project is under active development. Security fixes are applied to the default branch only.

## Reporting a Vulnerability

**Please do not open public GitHub issues for security vulnerabilities.**

Report privately by emailing the maintainers at:

- **we3ds.solution@gmail.com**

or by using GitHub’s private vulnerability reporting (Security tab → Report a vulnerability) if enabled for this repository.

### What to include

- Description of the vulnerability
- Steps to reproduce
- Potential impact
- Suggested remediation (if any)

### Response expectations

- Acknowledgement within **72 hours**
- Initial assessment within **7 days**
- Fix or mitigation timeline communicated after assessment

We appreciate responsible disclosure and will credit reporters (unless they prefer to remain anonymous).

## Known / Remaining Risks (as of 2026-10)

From the project README Security notes:

- Git history still contains previously committed secrets (connection string). Credentials must be rotated; history rewrite requires maintainer approval.
- No email confirmation, password reset, or 2FA yet.
- No rate-limiting on authentication endpoints.
- File uploads need antivirus / size review for large catalogs.

## Security Practices Already in Place

- Connection strings and secrets are not committed (use environment variables / user-secrets).
- Admin bootstrap is opt-in via environment variables only.
- Server-side price & stock validation on order creation (no client-trusted totals).
- Order ownership checks (no IDOR).
- Cookies: `HttpOnly`, `SameSite=Lax`, `Secure` in production.
- Explicit EF Core migrations (`MigrateAsync`) instead of `EnsureCreated`.
- HTTPS / HSTS expected in production.

## Production Checklist (Security-related)

See the full checklist in `README.md`. Key points:

1. Rotate any previously committed credentials.
2. Never commit secrets.
3. Use a secret manager for production connection strings.
4. Enable HTTPS + HSTS.
5. Review and approve migrations before applying to production.
