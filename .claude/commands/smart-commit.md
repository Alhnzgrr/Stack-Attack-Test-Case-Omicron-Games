---
description: Review changes, group them logically, propose a conventional commit message, and commit after approval. Never pushes.
allowed-tools: Bash(git status:*), Bash(git diff:*), Bash(git add:*), Bash(git commit:*)
---

Create a well-formed commit for the current changes.

Steps:
1. Run `git status` and `git diff` (and `git diff --staged`) to see all changes.
2. Group related changes into one logical commit. If the changes are clearly
   several unrelated units, say so and propose splitting into multiple commits.
3. Propose a Conventional Commits message: `type(scope): summary`, with a short
   body only if it adds real information.
4. Show the proposed message and the exact files, then **wait for approval**.
5. On approval, `git add` the relevant files and `git commit`. **Do not push.**
