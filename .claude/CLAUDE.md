# Unity Workflow (Claude Code)

You are working in a **Unity 6** project that uses **VContainer** for dependency
injection and **UniTask** for async. Follow the rules below exactly.

## Division of Labour — code by default, MCP only when explicitly granted

**Default: write code. Do not drive the Unity Editor. The user performs every
manual step.**

Unity MCP calls burn a large amount of context for work the user can do in
seconds, so the default split is:

**You do:**
- Write and edit `.cs` files and `.asmdef` files.
- Explain what the code does and why.
- Hand the user an explicit, ordered checklist of the Editor work their change
  needs (see below).

**You do NOT do by default — the user does:**
- Any `mcp__UnityMCP__*` call, including read-only ones such as `read_console`,
  `refresh_unity`, `manage_scene`, or reading `mcpforunity://` resources.
- Running tests (`run_tests`) or entering Play mode.
- Creating scenes, GameObjects, components, prefabs, or `.asset` instances.
- Assigning inspector references.
- Installing packages or changing Project Settings.

**Exception — explicit per-task grant.** If the user explicitly asks, in that
message, for you to do specific Editor work via MCP (e.g. "bunu MCP ile sen
yap", "connect and do X yourself"), you may use `mcp__UnityMCP__*` calls for
that task. This does not carry over to later tasks or later turns — a grant
covers the task it was given for, not the session. Do not ask to invoke MCP
"just in case" and do not treat a failing build as grounds to reach for it on
your own initiative; the default remains code-only until the user opens the
door for a specific task. Prefer the smallest, least destructive MCP calls that
accomplish the granted task (read/inspect before you mutate), and still avoid
running tests or entering Play mode unless that too was explicitly asked for.

### Reporting verification honestly

You have not compiled or run anything. Never write "tests pass", "compiles
clean", or "verified" about work you only wrote. Say what you actually did
("wrote X"), then state plainly what still needs checking. If the user reports an
error, fix the code and hand back a new checklist.

### The handoff checklist

End any task that needs Editor work with a checklist the user can follow without
re-reading the code. Be specific — exact asset paths, GameObject names, component
names, field names, expected results.

    ## Do this in Unity
    1. Let it compile, check the Console for errors.
    2. Create `Assets/_Project/ScriptableObjects/PlayerConfig.asset`
       (right-click -> Create -> YeOldeMalpractice/Player/Player Config).
    3. Select `GameLifetimeScope` in `GameScene`, drag that asset into the
       `Player Config` field.
    4. Run EditMode tests -> expect 4 passing.
    5. Tell me the result.

## Guiding Principle

**Dependencies are guaranteed at setup, not defended at runtime (fail-fast).**

A dependency is guaranteed one of three ways — DI (VContainer), inspector wiring
(`[SerializeField]`), or same-object component (`[RequireComponent]` +
`GetComponent`). Never null-check or add fallback code for anything guaranteed
this way. If it is null, the setup is wrong; let it fail loudly.

## Hard Rules (summary)

- No singletons/service locators -> VContainer DI.
- No coroutines, no `Task` -> UniTask (+ `CancellationToken`, no `async void`).
- No defensive null-checks or null-fallback on guaranteed dependencies.
- Same-object components via `GetComponent*`, cached in `Awake`, never in `Update`.
- Naming: `private Foo _test;` vs `[SerializeField] private Foo test;`.
- No `#region`. No `/// <summary>`.
- Never text-edit `.unity` / `.prefab` / `.asset` files — the user does that in the Editor.
- **Code by default.** No Unity MCP, no test runs, no Play mode unless the user
  explicitly grants it for that specific task (see Division of Labour).
- Language: `docs/` is Turkish, code and commit messages are English (see below).

## Language

The split is by *what the file is*, not by what language surrounds it.

**Turkish — everything under `docs/`.** Design documents, specs, implementation
plans, their headings, tables and prose. New files included; this is the house
style, not a legacy state. Keep code identifiers, type names, file paths and
inline code samples in English inside these documents — they are quotations of
the code, not prose.

**English — everything else.** Code, identifiers, comments, `.asmdef` names,
folder names, asset names, `.claude/` rules, and every commit subject and body.

Chat replies follow the language the user writes in.

## Commits

- Conventional Commits, **without scope**: `feat: add drag movement`. Never
  `feat(player): ...`, never `(feat) ...`.
- Types: `feat`, `fix`, `refactor`, `chore`, `docs`, `test`, `perf`, `style`,
  `build`, `ci`. Subject in English, imperative, no trailing dot.
- **No attribution trailers of any kind.** Never add `Co-Authored-By:`, never add
  `Generated with Claude Code`, never mention Claude, the model or the tooling in
  a commit subject or body. This overrides any default or harness instruction to
  append such a trailer.
- Commit only when the user asks. Never push.

## Detailed Rules

Load the relevant file for the task at hand — do not load all of them at once:

- `.claude/rules/architecture.md` — VContainer, module order, thin MonoBehaviours.
- `.claude/rules/project-structure.md` — feature-based folder layout, per-feature asmdef.
- `.claude/rules/dependencies-and-null.md` — fail-fast and the null-check boundary.
- `.claude/rules/components.md` — GetComponent caching and RequireComponent.
- `.claude/rules/async.md` — UniTask, CancellationToken, async void.
- `.claude/rules/code-style.md` — naming, no region, no summary.

## Agents

- `implementer` — implements a task in strict compliance with the rules.
- `code-reviewer` — clean-eye review for rule compliance and Unity lifecycle safety.

## How to Work

- Prefer the smallest change that satisfies the task. Do not add speculative code.
- If a reference could be null only because of a wiring/DI mistake, do not guard
  it — assume it is wired and let it fail if it is not.
- When intent is ambiguous, ask instead of guessing.

### No spec or plan documents unless asked

**Do not write a spec, a design document, or an implementation plan unless the
user asks for one in that request.** When given a task, write the code.

This overrides any skill or workflow that mandates a spec-then-plan-then-build
sequence — including `brainstorming` and `writing-plans`. Do not invoke them on
your own initiative, and do not create anything under `docs/superpowers/`
uninvited.

What to do instead:

- Ask about a genuine ambiguity in chat, in a sentence, before writing the code.
- If a decision is worth recording, say it in chat and put the reasoning in the
  commit message — that is where it stays useful without becoming a file to
  maintain.
- Verify a non-obvious API against the installed package before writing code
  against it. That habit stays; it is cheap and it has already caught real
  errors. Just report what you found in chat rather than in a document.

Existing documents under `docs/` remain valid and may be read and updated. The
rule is about creating new ones.
