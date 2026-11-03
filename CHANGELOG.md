# Changelog

## [1.2.2] - Community fork, Unity 6.6 compatibility

Community fork of `com.unity.polybrush`, published as `com.community.polybrush`.
Upstream (`com.unity.polybrush` 1.2.1) is declared for Unity 2022.3 and no longer
compiles on Unity 6.6, where several previously-warning APIs became hard errors.

### Fixed

- **CS0619 `Object.GetInstanceID()`** — obsolete in Unity 6.6 ("Use GetEntityId instead").
  All usages now go through the new helpers `EditableObject.GetObjectId()`,
  `GetMeshInstanceName()` and `ObjectFromId()`, which use `Object.GetEntityId()` on
  Unity 6.6+ and fall back to `GetInstanceID()` on older versions.

- **CS0619 `AssetDatabase.GetAssetPath(int)`** — obsolete in Unity 6.6.
  `Editor/Classes/PolyAsset.cs` now calls the still-valid `GetAssetPath(Object)` overload.

- **CS0619 `EditorUtility.InstanceIDToObject(int)`** — obsolete in Unity 6.6
  ("InstanceIDToObject(int) is obsolete. Use EditorUtility.EntityIdToObject instead").
  Replaced with `EditorUtility.EntityIdToObject(EntityId)` on Unity 6.6+.

- **UAC0005 `AppDomain.CurrentDomain.GetAssemblies()`** — raised to an error by the
  Unity 6.6 analyzer because it can return already-unloaded assemblies.
  `Editor/Utility/ReflectionUtility.cs` now uses the supported replacement
  `UnityEngine.Assemblies.CurrentAssemblies.GetLoadedAssemblies()`.
  Note: the message also suggests `CurrentAssemblies.GetLoadedAssemblies()`, and the
  type lives in `UnityEngine.Assemblies`, not `UnityEditor` — it is only listed as
  `CurrentAssemblies` without a namespace in the diagnostic.

### Changed — mesh instance id is now 64-bit

Polybrush encoded the owning GameObject's id into the mesh name as
`"PolybrushMesh" + id` and recovered it with `int.TryParse`. `EntityId` raw values no
longer fit in 32 bits (observed values such as `462894395366476`), so:

- `EditableObject.GetMeshId()` now returns `long` instead of `int`.
- The "not a Polybrush mesh" sentinel changed from `-1` to `0`
  (`k_InvalidMeshId`), since `EntityId.None.GetRawData() == 0` and `IsValid() == false`.
- The `EntityId -> int` implicit conversion exists but is **already obsolete**
  ("EntityId will not be representable by an int in the future"), so it is deliberately
  not used.

No migration is required: the encoding/decoding round-trip was verified over 300
GameObjects with zero failures, and projects with no Polybrush-managed meshes are
unaffected (the mesh name prefix is only written when a mesh is actually painted).

### Notes

- Assembly names are unchanged (`Unity.Polybrush`, `Unity.Polybrush.Editor`).
  **Do not install this fork alongside `com.unity.polybrush`** — the duplicate
  `Unity.Polybrush.Editor` asmdef produces
  "Assembly with name 'Unity.Polybrush.Editor' already exists".
- `unity` in `package.json` was raised to `6000.0`, from the upstream `2022.3`.
  The `#if UNITY_6000_6_OR_NEWER` guards keep the code compatible with older
  versions, so this can be lowered if you need to support them.
