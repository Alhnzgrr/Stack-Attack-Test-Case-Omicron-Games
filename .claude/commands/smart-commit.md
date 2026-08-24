---
description: Review changes, group them logically, propose a conventional commit message, and commit after approval. Never pushes.
allowed-tools: Bash(git status:*), Bash(git diff:*), Bash(git add:*), Bash(git commit:*)
---

Create a well-formed commit for the current changes.

Steps:
1. Run `git status` and `git diff` (and `git diff --staged`) to see all changes.
2. Group related changes into one logical commit. If the changes are clearly
   several unrelated units, say so and propose splitting into multiple commits.
3. Propose a Conventional Commits message: `type: summary`, with a short body
   only if it adds real information.
   - **No scope in parentheses.** Write `feat: add drag movement`, never
     `feat(player): add drag movement` and never `(feat) add drag movement`.
   - Type is one of `feat`, `fix`, `refactor`, `chore`, `docs`, `test`, `perf`,
     `style`, `build`, `ci`.
   - Subject in English, imperative, lowercase after the colon, no trailing dot.
4. **Never add co-author or attribution trailers.** No `Co-Authored-By:`, no
   `Generated with Claude Code`, no tool or model mention anywhere in the
   subject or body. The commit is authored by the user alone.
5. Show the proposed message and the exact files, then **wait for approval**.
6. On approval, `git add` the relevant files and `git commit`. **Do not push.**
