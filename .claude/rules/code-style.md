# Code Style

## Naming

- Pure private field — leading underscore:

        private Foo _test;

- Serialized private field — no underscore (the missing underscore signals
  "exposed in the inspector"):

        [SerializeField] private Foo test;

## Forbidden

- **No `var`.** Always write the explicit type, including inside local scopes
  and `for` headers:

        float worldDelta = normalizedDelta * width;
        SpriteRenderer plate = _plates[i];
        for (int i = 0; i < _plates.Length; i++)

  This also applies to `foreach` and to types that look obvious from the right
  hand side. `out` declarations keep their explicit type as usual
  (`TryGetComponent(out IDamageable target)`).

- **No `#region` / `#endregion`.**
- **No `/// <summary>` XML documentation comments.** Code explains itself through
  clear names. Add a plain `//` comment only when intent is genuinely non-obvious.
