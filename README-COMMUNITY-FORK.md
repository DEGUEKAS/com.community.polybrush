# Polybrush — Community Fork (Unity 6.6 compatible)

Fork of `com.unity.polybrush` (upstream `needle-mirror`, v1.2.1) fixed to compile on
Unity 6.6, where several APIs became hard errors.

Repo: https://github.com/DEGUEKAS/com.community.polybrush.git
Package name: `com.community.polybrush`
Base version: `1.2.2`

---

## What was broken

Upstream declares `"unity": "2022.3"` and produced 13 console errors on Unity 6.6:

| # | Error | Cause |
|---|-------|-------|
| 4 | `Assembly with name 'Unity.Polybrush.Editor' already exists` | Package installed twice (Git + `Assets/`) |
| 7 | `CS0619` on `GetInstanceID()` | Obsolete in 6.6 → `GetEntityId()` |
| 2 | `CS0619` on `AssetDatabase.GetAssetPath(int)` | Obsolete in 6.6 → `GetAssetPath(EntityId)` |
| 2 | `CS0619` on `EditorUtility.InstanceIDToObject(int)` | Obsolete in 6.6 → `EntityIdToObject()` |
| 1 | `UAC0005` on `AppDomain.GetAssemblies()` | Analyzer rule became an error |

---

## Files changed

Only 4 source files, all under `Editor/`:

```
Editor/Classes/PolyAsset.cs          AssetDatabase.GetAssetPath(int) → GetAssetPath(this)
Editor/Classes/EditableObject.cs     int → long ids; new helper trio
Editor/Interface/HierarchyChanged.cs uses the new helpers (no #if needed)
Editor/Utility/ReflectionUtility.cs  CurrentAssemblies.GetLoadedAssemblies()
package.json                         name/version/repository
CHANGELOG.md                         full write-up
```

All Unity-6.6-specific code is wrapped in `#if UNITY_6000_6_OR_NEWER`, with the
original implementation preserved in the `#else` branch, so the package still builds
on Unity 2022.3 – 6.5.

---

## Installation

### 1. Push the patched files to your fork

Copy the contents of `patched/` over your clone of
`https://github.com/DEGUEKAS/com.community.polybrush.git`, then:

```bash
git add -A
git commit -m "fix: Unity 6.6 compatibility (EntityId APIs, UAC0005)"
git push
```

### 2. Point the project at the fork

`Packages/manifest.json` (already applied in this project):

```json
"com.community.polybrush": "https://github.com/DEGUEKAS/com.community.polybrush.git"
```

### 3. Remove the old package

The fork uses the **same assembly names** (`Unity.Polybrush`,
`Unity.Polybrush.Editor`) as upstream. Installing both at once reproduces
`Assembly with name 'Unity.Polybrush.Editor' already exists`, so the upstream
`com.unity.polybrush` entry must be gone — it already is, the manifest line was
replaced rather than added.

Then force a re-resolve so Unity drops the cached upstream copy:

- **Window → Package Manager →** gear icon → **Resolve Package Manager**
  (or delete `Library/PackageCache/com.unity.polybrush@*` with Unity closed).

### 4. Verify

Console should show **0 errors**. Open **Tools → Polybrush → Polybrush Window** and
paint an object to confirm a `PolybrushMesh` component is created and the mesh is
renamed to `PolybrushMesh<id>`.

---

## Rollback

The original manifest is saved at:

```
_polybrush_fork_backup/manifest.json.original
```

Restore it, remove the `com.community.polybrush` entry, and run
Resolve Package Manager again.

---

## To pin a revision

Git dependencies can pin a commit, which is safer than tracking a branch:

```json
"com.community.polybrush": "https://github.com/DEGUEKAS/com.community.polybrush.git#<commit-sha>"
```
