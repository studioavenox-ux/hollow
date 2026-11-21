# HOLLOW — Assets/_HOLLOW

Asset folder for **HOLLOW** (AVENOX Games), a PC-first, first-person psychological thriller. The Unity project root is the **repository root**, not `Assets/_HOLLOW`.

## Open in Unity

1. Follow `Docs/Repository_Setup.md` to clone the branch and open the repository root in **Unity 6000.3.0f1** through Unity Hub.
2. Let Unity import the project and review the Console. `Library/` is ignored; asset `.meta` files are **tracked** and must be kept with their assets. The checked-in metadata was pre-generated outside Unity and still needs Editor-import validation.
3. After compilation, the optional **HOLLOW → Bootstrap Vertical Slice Scene** menu attempts to create `Assets/_HOLLOW/Scenes/VerticalSlice.unity`. It refuses to overwrite an existing scene at that path. The result is an **incomplete, untested greybox**, not a playable slice.
4. Set Active Input Handling to **Both**, and import TMP Essential Resources when prompted. The scripts intend WASD/mouse, E, Tab, and Esc, but UI/data/geometry wiring is incomplete. See `Docs/Bug_Risk_Register.md` before testing.

## Structure (Master §15.2)

- `Scripts/Core` — GameManager, SaveSystem, SceneTransitionManager, StoryFlags
- `Scripts/Player` — FirstPersonController
- `Scripts/Interaction` — Interactable, InteractionRaycaster, InspectableObject
- `Scripts/Dialogue` — DialogueManager, SubtitleDisplay, DialogueData
- `Scripts/Story` — ObjectiveManager, EvidenceManager, JournalManager
- `Scripts/UI` — HUDManager, JournalUI, EvidenceViewerUI, PauseMenu
- `Scripts/Editor` — SceneBootstrap (greybox generator)
- `Data/` — JSON story drafts (dialogue, evidence, objectives). Dialogue/objective loaders require assigned TextAssets; evidence JSON has no runtime loader yet.
- `Scenes/`, `Prefabs/`, `Art/`, `Audio/`, `Materials/`, `UI/`, `Cinematics/` — production folders (greybox placeholders for now).

## Canon guardrails

- Prakash's message is a **placeholder** (`Data/Evidence/Prakash_Message_PLACEHOLDER.json`) — do not treat as canon.
- Village name, buried secret, deity, ending are **OPEN** — see `Docs/Story_Decision_Register.md`.
- See `Docs/Technical_Decision_Register.md` for pipeline/architecture decisions.
