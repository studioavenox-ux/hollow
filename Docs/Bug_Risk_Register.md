# HOLLOW — Bug & Risk Register

## Phase 1.1 static findings (21 Nov 2026)

These findings are from source/configuration review, **not** an Editor run. "Source corrected" means the likely defect was edited, not that Unity has compiled or verified the fix. Track Editor results and retests separately.

| ID | System | Finding and impact | Severity | Status / next verification |
|---|---|---|---|---|
| B-01 | C# compilation | Namespace-level `private ObjectiveListWrapper` and null-conditional property assignments in `DialogueManager` were invalid for the intended C# version. | Blocker | **Source corrected**, awaiting Unity compilation. |
| B-02 | Editor bootstrap | `NewScene` used the wrong enum and `SaveScene` omitted the intended path; the player camera also lacked the `MainCamera` tag. | High | **Source corrected**; bootstrap now refuses to overwrite an existing destination and asks about unsaved scenes. Test it in Unity. The scene is not added to Build Profiles. |
| B-03 | UI / scene | Generated canvas has only manager components, with no TextMeshPro labels, panels, button/event setup, or serialized UI references. Prompts, subtitles, journal, evidence and pause UI cannot display as intended. | Blocker | **Open** — create and wire UI after this validation gate; import TMP Essential Resources. |
| B-04 | Story data / progression | Dialogue and objective `TextAsset` fields are never assigned by the bootstrap. `InspectableObject` has no evidence ID/content set, and no code loads the evidence JSON. Photograph/message are not separate inspectables; objective transitions to the meeting and first lead are not wired. | Blocker | **Open** — assign/link content and test the full sequence later; JSON parsing alone does not prove gameplay. |
| B-05 | Greybox geometry | The post office is a solid cube around the Gopinath and package positions, so its collider obstructs access. | High | **Open** — inspect and correct geometry in the Editor. |
| B-06 | Input | Code uses legacy `UnityEngine.Input` only. `ProjectSettings.asset` / Input Manager settings are absent, so the needed Active Input Handling and axes are not committed. | High | **Open** — in Player settings select **Both** (or legacy Input Manager), verify Horizontal/Vertical/Mouse X/Mouse Y axes, restart if prompted. |
| B-07 | Scene transitions | There is no committed scene or build-scene list, and `PauseMenu` requests a nonexistent `MainMenu`; scene loading does not automatically restore saved state after a load. | High | **Open** — create/verify scenes and build configuration before enabling menu/transition actions. |
| B-08 | Checkpoint save/load | No startup/UI path loads a checkpoint in the bootstrap; a save with no player silently records the manager's transform. Restoring collected IDs does not rebuild `JournalManager` discoveries, and scene/camera pitch are not saved. | High | **Open** — test round trips, rebuild journal state and define safe missing-player/scene handling. |
| B-09 | Manager lifetimes | `GameManager` and its save/transition components persist across scenes, but their player/UI/other-manager references are scene-scoped and not rebound. Duplicate transition components can also create persistent fade canvases. | High | **Open** — define ownership and test reload/transition lifecycle. |
| B-10 | Modal input / timing | The same E press that starts dialogue can advance multiple lines in the same frame; the evidence viewer can similarly close on its opening press depending on update order. Escape can both close evidence and toggle pause, and pause does not comprehensively gate look/dialogue/journal input. | High | **Open** — gate input by modal state and verify ordering in Play mode. |
| B-11 | Interaction | The raycaster uses its own `maxDistance` but does not enforce each `Interactable.InteractionRange`; footsteps can fire more than once near a bob peak and have no audio subscriber. | Medium | **Open** — verify ranges and event cadence when wiring gameplay/audio. |
| B-12 | Project / assets | No lockfile, URP pipeline asset/graphics assignment, or full set of Unity-generated project settings is committed. Pre-generated `.meta` GUIDs/importers have not been validated by the Editor. | High | **Open** — first Editor owner validates import, commits reviewed lockfile/settings/metadata, and configures URP if approved. |

## Risks

| ID | Risk (Master §18.2) | Impact | Mitigation in scaffold | Owner / Next step |
|---|---|---|---|---|
| R-01 | Story scope expands too early | Delays playable game | Slice-only scope; no palace/forest/mythology built. OPEN story items kept as placeholders. | Adi: approve S-02/S-04 before content lock |
| R-02 | Mystery becomes confusing | Player frustration | Reveal ladder (§9.3), objective tracking and clue redundancy are planned; objective data exists but UI/progression are not wired. | Playtest slice for objective clarity |
| R-03 | Cultural details inaccurate | Authenticity harm | No real religious/cultural content invented; Odia text deferred until verified; fictional placeholder naming. | Research pass before art production |
| R-04 | Environment production too large | Cost/schedule | Compact connected zones, greybox first, streaming deferred (T-07). | Profile greybox before expanding |
| R-05 | Horror relies on jumpscares | Emotional weight loss | No jumpscares in scaffold; tension via sound/lighting/NPC behaviour is planned (§12.2), not yet delivered. | Design review at polish phase |
| R-06 | Technical systems overbuilt | Delays first playable | Minimal separated systems; no combat/crafting/multiplayer (T-09). | Keep slice lean |
| R-07 | Performance on target hardware unknown | Unplayable build | URP recommended but untested (T-01); no min-spec promised (T-10). | Profile on real target machine; record results |
| R-08 | **Scaffold uncompiled** — further C# or API errors may appear on first Unity open | Blocks Phase C | Reviewed all 20 scripts; syntax parsing and targeted fixes are not a compiler or API check. | First Editor open: fix + log all errors here |
| R-09 | Asset metadata was pre-generated outside Unity | Incorrect importer settings or GUID churn could break future references | Unique GUIDs and standard importer shapes checked statically. | One Editor owner validates import and commits any reviewed changes before scenes are shared. |
| R-10 | Package graph and settings have never been resolved by Unity | Editor import can fail or choose different transitive versions | Manifest compared with Unity 6.3 documentation; no packages lock exists yet. | Resolve on the target Editor, inspect Console, and commit the reviewed lock/settings. |

## Quality gates (Master §18.1) — slice checklist

- [ ] Story: slice moves central mystery forward (package → first lead)
- [ ] World: village entrance + post office believable, inhabited feel
- [ ] Gameplay: next action always clear
- [ ] Horror: tension from context, not shocks
- [ ] Performance: profiled on target hardware, results recorded
- [ ] Cinematics: continuity + subtitles correct (opening transition)
- [ ] Accessibility: subtitle size, volume, sensitivity, rebinding present
