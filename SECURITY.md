# Security Policy

## Supported Versions

| Version | Supported |
| ------- | --------- |
| `main` / latest | :white_check_mark: |
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

## Known / Remaining Risks (as of 2026-10-11)

- **Git history** may still contain a previously committed DB connection string. Treat those credentials as compromised and **rotate** them. Full history rewrite requires a maintainer force-push — see `docs/HISTORY-PURGE.md` and issue #13.
- File uploads need antivirus / size review for large catalogs.

## Security Practices Already in Place

- Connection strings and secrets are not committed in the working tree (environment variables / user-secrets).
- Admin bootstrap is opt-in via environment variables only.
- Server-side price & stock validation on order creation (no client-trusted totals).
- Order ownership checks (no IDOR).
- Cookies: `HttpOnly`, `SameSite=Lax`, `Secure` in production.
- Explicit EF Core migrations (`MigrateAsync`) instead of `EnsureCreated`.
- Email confirmation + password reset flows; rate limiting on auth/checkout POSTs; TOTP 2FA available.
- **CI secret scan:** official Gitleaks CLI on every push/PR (working tree; no org license).
- HTTPS / HSTS expected in production.

## Production Checklist (Security-related)

See the full checklist in `README.md`. Key points:

1. Rotate any previously committed credentials.
2. Never commit secrets.
3. Use a secret manager for production connection strings.
4. Enable HTTPS + HSTS.
5. Review and approve migrations before applying to production.
6. After rotation, optionally purge git history per `docs/HISTORY-PURGE.md`.
