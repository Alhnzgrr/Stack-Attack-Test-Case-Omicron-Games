# Dependencies & Null

**Guiding principle:** dependencies are guaranteed at setup, not defended at
runtime (fail-fast).

A dependency is guaranteed one of three ways:
1. **DI (VContainer)** — injected services.
2. **Inspector wiring** — `[SerializeField]` references.
3. **Same-object component** — `[RequireComponent]` + `GetComponent`.

## Rules

- **Do not null-check anything guaranteed by the three paths above.** If it is
  null, the setup is wrong. Let it throw so the bug is visible and gets fixed.
- **Never write null-fallback code.** Forbidden:

        if (target == null) target = FindObjectOfType<Target>();
        if (rb == null) rb = GetComponent<Rigidbody>();

  A missing reference is a wiring bug, not a runtime branch.

- **Null-checks ARE allowed for genuinely optional / runtime-dynamic values** —
  this is correct code, not defensive code:

        if (Physics.Raycast(ray, out var hit) &&
            hit.collider.TryGetComponent(out IDamageable target))
            target.Damage(10);

  Examples: raycast/overlap results, `TryGetComponent`, dictionary/collection
  lookups, save/deserialized data, `Camera.main`, optional gameplay targets.
