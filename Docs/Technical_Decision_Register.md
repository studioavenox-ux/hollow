# HOLLOW — Technical Decision Register

Status key: **DECIDED** = chosen (not necessarily validated) · **PARTIAL** = scaffolding only · **RECOMMENDED** = needs Editor validation · **OPEN** = needs implementation/test/approval.

| ID | Decision | Status | Rationale | Validation needed |
|---|---|---|---|---|
| T-01 | Rendering pipeline: **URP** (Universal Render Pipeline) | RECOMMENDED | PC-first semi-realistic 3D; URP is a candidate for balancing visuals and performance. The package is listed, but no pipeline/renderer assets or Graphics/Quality assignment exist yet. | Configure and test on target hardware; record FPS/frame time before locking. |
| T-02 | Packages: URP 17.3.0, Input System 1.16.0, Timeline 1.8.9, Unity UI 2.0.0 (includes TMP), Test Framework 1.6.0 | PARTIAL (manifest) | Unity 6 uses TMP via uGUI rather than the legacy standalone TMP package. Direct versions and engine modules are declared; there is no resolved package lock yet. | Resolve in 6000.3.0f1, import TMP Essential Resources, review and commit `Packages/packages-lock.json`; adjust versions only after Editor verification. |
| T-03 | Input: legacy Input API for the slice (WASD + mouse look, E interact, Tab journal, Esc pause); Input System package installed for future rebinding | PARTIAL (code) | No Input Actions or rebinding exist yet. **Editor setting required:** Edit → Project Settings → Player → Other Settings → Active Input Handling = **Both**; the relevant project settings file is not committed. | Confirm legacy axes, restart if prompted, and playtest; later migrate to Input Actions + rebinding UI. |
| T-04 | Architecture: separated systems, no god-manager | PARTIAL | Player / Interaction / Dialogue / Story / Save / Scene-transition are separate classes. `GameManager` holds references, but persistent vs. scene-scoped ownership is not yet safe. | Compile and verify lifecycle/rebinding after scene reload (B-09). |
| T-05 | Data-driven story content (JSON) | PARTIAL | Draft dialogue, evidence and objectives live in `Assets/_HOLLOW/Data/`. Dialogue/objective loaders exist but their assets are not wired; evidence JSON has no runtime loader. | Assign content and validate the evidence pipeline in Play mode; preserve explicit placeholders. |
| T-06 | Save: JSON checkpoint save to `Application.persistentDataPath` | PARTIAL (code) | Saves a serializable flag-entry list, evidence IDs, objective ID and player transform; missing-player fallback and journal restoration are unsafe/incomplete. | Test save → reload → resume and failure handling in Editor (B-08); no save success is claimed. |
| T-07 | Scene strategy: single `VerticalSlice` scene + `SceneTransitionManager` (fade) | PARTIAL (code) | Small environment first (§15.4). There is no committed scene/build list or spawn point; transitions can pre-save but do not automatically restore state or rebind scene references. | Create build configuration and test transition, reload and state preservation (B-07/B-09). |
| T-08 | Greybox via Editor bootstrap script (not hand-authored scene YAML) | PARTIAL | `SceneBootstrap.cs` attempts to generate a scene in-Editor and refuses to overwrite an existing destination; geometry, data and UI are incomplete. | Run only after import/compile, then inspect the scene and the known blockers. |
| T-09 | No combat / crafting / multiplayer / large open world in slice | DECIDED (scope) | Master §2.3 non-goals + §10.4. Slice proves explore→observe→investigate→interact only. | Revisit only if slice playtest proves a need. |
| T-10 | Minimum PC spec | OPEN | Must not be promised before profiling (§15.4). | Profile on actual target/test machines; then record here. |
| T-11 | Cinematic workflow (Timeline vs. custom) | OPEN | Master §13.1 requires in-engine cinematics "where validated". Timeline is the default recommendation. | Validate Timeline + subtitle sync in prototype. |
| T-12 | Asset GUIDs / `.meta` files | PARTIAL | Standard-format metadata with unique GUIDs was pre-generated for the 50 existing visible asset files and folders; `.gitignore` allows them. No Unity import has occurred. | First Editor owner validates importer settings/GUID stability and commits reviewed metadata changes before sharing scenes. |

## Package compatibility evidence (static only)

- The Unity 6000.3.0f1 release identifies the revision `d1870ce95baf` and lists Input System 1.16.0 [1](https://unity.com/releases/editor/whats-new/6000.3.0f1). URP 17.3.0 targets the 6000.3 line [2](https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.universal/CHANGELOG.md).
- Unity's uGUI 2.0 documentation includes TextMesh Pro and requires importing TMP Essential Resources [1](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/index.html). The Unity Test Framework 1.6 changelog identifies version 1.6.0 [1](https://docs.unity3d.com/Packages/com.unity.test-framework@1.6/changelog/CHANGELOG.html); Timeline 1.8.9 was in the 6000.3 package line before 6000.3.3f1 updated it [4](https://unity.com/releases/editor/whats-new/6000.3.3f1).
- This evidence supports the selected versions but does **not** establish that the combined dependency graph resolves or works in this checkout. Unity Package Manager has not run and there is no lockfile.

## Naming / folder conventions (Master §15.2)

`Assets/_HOLLOW/{Art,Audio,Materials,Prefabs,Scenes,Scripts/{Core,Player,Interaction,Dialogue,Story,UI,Editor},UI,Data,Cinematics}`. Classes use `Hollow.*` namespaces per system. No `Resources/` folder until a system genuinely needs it.
