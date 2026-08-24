# Architecture Rules

- **No singletons, no service locators.** Compose dependencies with **VContainer**.
- Resolve services through constructor/method injection or `LifetimeScope`
  installers. Do not expose global static access points.
- **Module order:** Interface -> Service -> Config -> Installer -> Events.
  - `Interface`: the contract the rest of the code depends on.
  - `Service`: the implementation, plain C# where possible.
  - `Config`: serialized/`ScriptableObject` data the service needs.
  - `Installer`: VContainer registration.
  - `Events`: signals the service raises/consumes.
- **Keep MonoBehaviours thin.** Real logic lives in plain C# classes that can be
  tested without a scene. MonoBehaviours only adapt Unity lifecycle to services.
