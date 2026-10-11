# Git history secret purge (Issue #13)

## Status

- **Working tree:** no live production credentials are committed (connection strings use LocalDB placeholders / env vars).
- **CI:** Gitleaks CLI scans the working tree on every push/PR (no org license required).
- **History:** older commits may still contain a previously committed DB connection string. Those credentials must be treated as **compromised and rotated** regardless of whether history is rewritten.

## Why force-push needs a human maintainer

Rewriting history changes every commit SHA and requires a coordinated `git push --force` to the default branch. Only an org admin/maintainer should run it after the team agrees and credentials are rotated.

## Recommended procedure

1. **Rotate** every secret that ever appeared in the repo (DB password, any reused keys).
2. Ensure all collaborators have pushed/merged open work.
3. On a clean clone:

```bash
git clone --mirror https://github.com/we3ds-solution/Shop-Zonta-Mvc.git
cd Shop-Zonta-Mvc.git
# Install git-filter-repo: https://github.com/newren/git-filter-repo

# Example: remove historical path content for appsettings that held secrets
git filter-repo --path market-mvc/appsettings.json --invert-paths
# Or replace strings with a expressions file — prefer path-based if the whole file was leaked.

# Re-add a clean appsettings.json from the current tip after filter if needed.
```

4. Force-push all branches/tags the team uses (coordinate downtime):

```bash
git push --force --all origin
git push --force --tags origin
```

5. Ask every collaborator to **re-clone** (or hard-reset to the new history).
6. Confirm CI Gitleaks still passes on the new tip.
7. Optionally switch the CI scan from `--no-git` to full git history once the purge is verified.

## Already done in-repo

- Secrets removed from current `appsettings.json` / env templates.
- Gitleaks in CI (working tree) + `.gitleaks.toml` allowlists for test fixtures.
- `.gitignore` excludes `App_Data/mail` and `*.eml`.

Closing this issue after the maintainer completes steps 1–6 above.
