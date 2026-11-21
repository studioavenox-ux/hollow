# HOLLOW — Windows repository setup

This checkout is a **scaffold, not a playable build**. Unity import, package resolution, compilation and playtesting have not been performed in the preparation environment. Known blockers are in `Docs/Bug_Risk_Register.md`.

## 1. Install prerequisites

1. Install [Git for Windows](https://git-scm.com/download/win) and [Unity Hub](https://unity.com/download). Sign in to Unity Hub and activate an appropriate Unity license.
2. In Unity Hub → **Installs** → **Install Editor**, install **Unity 6.3 LTS, version `6000.3.0f1`**. If it is not listed, open Unity's 6000.3.0f1 release page [1](https://unity.com/releases/editor/whats-new/6000.3.0f1) and use its **Install** link to open Unity Hub. The official revision is `d1870ce95baf`.
3. Add **Windows Build Support (IL2CPP)** and an IDE such as Visual Studio if you need to build or edit code; they are not required merely to open the project. Do not use a different Editor patch without explicitly reviewing the project/package changes it may make.

## 2. Clone the branch

Open **PowerShell**. The commands below place a new `hollow` folder under your Windows user directory:

```powershell
cd $HOME
git clone --branch arena/895a51a9-hollow https://github.com/studioavenox-ux/hollow.git hollow
cd .\hollow
git status --short --branch
```

If `hollow` already exists, choose another **new** folder name instead of overwriting or deleting the existing directory. If repository access requires authentication, use Git for Windows' normal secure GitHub sign-in flow.

## 3. Add and open in Unity Hub

1. In Hub, choose **Projects** → **Add** → **Add project from disk**.
2. Select the **repository root** (`%USERPROFILE%\hollow`, unless you chose another folder). It contains `Assets`, `Packages` and `ProjectSettings`; do **not** select `Assets/_HOLLOW`.
3. Open the project with **6000.3.0f1**. Allow Package Manager and asset import to finish, then open **Window → General → Console**. Record errors and warnings before trying the bootstrap. The project has never been opened in Unity in this environment, so package and compiler failures remain possible.
4. In **Edit → Project Settings → Player → Other Settings**, set **Active Input Handling** to **Both** and accept an Editor restart if prompted. The scaffold uses the legacy `UnityEngine.Input` API and expects the `Horizontal`, `Vertical`, `Mouse X`, and `Mouse Y` axes in the Input Manager.
5. Import **Window → TextMeshPro → Import TMP Essential Resources** if they are not already present. Review any import/overwrite prompts rather than replacing existing assets.

## 4. Review generated files and optional greybox

- Asset `.meta` files are **tracked**; keep each file and its GUID with its asset. The current 50 metadata files were pre-generated outside Unity and have not been Editor-validated. Review any changes Unity makes to them, as well as newly generated project settings and `Packages/packages-lock.json`. `Library/`, `Temp/`, `Logs/` and build output are intentionally ignored. Do not discard or replace existing assets to get a clean status.
- Once the Console has no blocking compilation errors, **HOLLOW → Bootstrap Vertical Slice Scene** may be used to create `Assets/_HOLLOW/Scenes/VerticalSlice.unity`. It prompts for unsaved scenes and refuses to overwrite an existing scene at that path. It does **not** add the scene to a build profile.
- The resulting greybox is incomplete: UI/data references are not wired, the post office collider obstructs its contents, and save/load and scene transitions are unverified. It is not an end-to-end playable slice. See the bug register before any test.
- Optional repository-only check, if Python 3 is installed: `py -3 .\Tools\validate_scaffold.py`. It reads files and runs Git ignore checks; it is **not** Unity compilation, package resolution or a playtest.

After the first Editor session, run `git status --short` to inspect changes. Coordinate with the team before committing reviewed settings, generated lockfiles, scenes and their `.meta` files; never commit `Library/` or other caches.
