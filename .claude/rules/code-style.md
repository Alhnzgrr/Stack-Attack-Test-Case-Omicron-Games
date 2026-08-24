# Code Style

## Naming

- Pure private field — leading underscore:

        private Foo _test;

- Serialized private field — no underscore (the missing underscore signals
  "exposed in the inspector"):

        [SerializeField] private Foo test;

## Forbidden

- **No `#region` / `#endregion`.**
- **No `/// <summary>` XML documentation comments.** Code explains itself through
  clear names. Add a plain `//` comment only when intent is genuinely non-obvious.
