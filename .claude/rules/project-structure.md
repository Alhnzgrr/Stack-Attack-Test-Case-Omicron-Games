# Project Structure

Feature-based layout. Each feature is a self-contained folder; the physical files
map one-to-one to the module order (Interface -> Service -> Config -> Installer ->
Events).

## Root skeleton

`Assets/_Project/` is the owned root. Third-party packages stay at the `Assets/`
root and are never mixed in.

    Assets/_Project/
      Core/
        Core.asmdef
        RootLifetimeScope.cs
      Features/
      ScriptableObjects/
      Scenes/
      Prefabs/

## Feature skeleton

    Assets/_Project/Features/Player/
      Player.asmdef
      IPlayerService.cs      Interface  — the contract
      PlayerService.cs       Service    — plain C# implementation
      PlayerConfig.cs        Config     — ScriptableObject data
      PlayerInstaller.cs     Installer  — VContainer registration
      PlayerEvents.cs        Events     — signals raised/consumed

Not every feature needs all five files — create only the roles the feature
actually uses (e.g. a stateless service may have no `Config` or `Events`). Keep
the naming and order consistent for the ones that exist.

## Assembly definitions (asmdef)

- `Core.asmdef` references no feature. Dependencies point **inward**: features
  depend on `Core`, never the reverse.
- Each `Feature.asmdef` references **`Core`, `VContainer`, `UniTask`** — and
  **never another feature**. Features are isolated. If two features must talk,
  they do it through an interface or event defined in `Core`.
- `.asmdef` is a JSON file, not `.asset`/`.unity`/`.prefab` — write it directly.
  Unity generates the accompanying `.meta`; never hand-write `.meta` files.

## ScriptableObject boundary

Writing `PlayerConfig.cs` (the C# `ScriptableObject` class) is normal code and is
allowed. Creating the `.asset` **instance** of that ScriptableObject is a Unity
Editor action the **user** performs — never text-edit the `.asset` file, and never
create it yourself via MCP. Write the class, then put the asset creation on the
handoff checklist.

## When asked to "set up the folder structure"

- **Empty / new project:** create the root skeleton above.
- **Existing project, a named feature** ("set up the Player feature"): create that
  feature's folder with its `.asmdef` and minimal stubs for the roles it needs.
- Never overwrite what already exists — only add what is missing.
