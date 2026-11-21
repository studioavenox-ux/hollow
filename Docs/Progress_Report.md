# HOLLOW — Development Progress Report

- **Studio:** AVENOX Games
- **Project:** HOLLOW — *Some memories should remain buried.*
- **Master document:** HOLLOW_Master_Development_Document_v1.0.pdf (v1.0, 21 Nov 2026)
- **Engine target:** Unity 6.3 LTS, PC-first, first-person
- **Branch:** `arena/895a51a9-hollow` (from `main` @ `0c34b7b`)
- **Last updated:** 21 Nov 2026 — Phase 1.1 static repository/scaffold validation; Unity verification pending

---

## 1. What has been done

### Phase A — Audit and Planning (COMPLETE)
- [x] Read and cross-checked the full 24-page Master Development Document v1.0 against the approved story foundation in the development brief. No contradictions found; the brief's story facts match the PDF's APPROVED CANON sections.
- [x] Audited the sandbox/development environment (OS, CPU, RAM, disk, GPU, Unity presence, .NET, git state). See `Docs/Environment_Audit.md`.
- [x] Inspected the existing repository: only `README.md` + Unity-standard `.gitignore` + initial commit `0c34b7b`. No existing Unity project files to overwrite. Working tree was clean.
- [x] Produced the required first-response package: identity summary, story facts, unresolved decisions, readiness assessment, architecture, vertical-slice plan, blockers (see session response + `Docs/Vertical_Slice_Plan.md`).
- [x] Created decision registers: `Docs/Technical_Decision_Register.md`, `Docs/Story_Decision_Register.md`, `Docs/Bug_Risk_Register.md`, `Docs/Milestones.md`.

### Phase B — Project Foundation (STARTED — scaffold only)
- [x] Created the `Assets/_HOLLOW/` folder structure per Master §15.2.
- [x] Added `ProjectSettings/ProjectVersion.txt` and `Packages/manifest.json` (URP + Input System + Timeline + TextMeshPro + Test Framework — see Technical Register T-01/T-02).
- [x] Implemented modular C# systems (not yet compiled in Unity — see Blockers): Core (GameManager, SaveSystem, SceneTransitionManager, StoryFlags), Player (FirstPersonController), Interaction (Interactable, InteractionRaycaster, InspectableObject), Dialogue (DialogueManager, SubtitleDisplay), Story (ObjectiveManager, EvidenceManager, JournalManager), UI (HUDManager, JournalUI, EvidenceViewerUI, PauseMenu).
- [x] Added JSON draft content: Gopinath first-meeting dialogue, Prakash package evidence (photograph + PLACEHOLDER message), and vertical-slice objectives. Dialogue/objective assets are not assigned and evidence has no runtime loader.
- [x] Added `Assets/_HOLLOW/Scripts/Editor/SceneBootstrap.cs` — an untested Editor menu tool intended to generate a greybox scene with player, village entrance, post office, lighting and fog. No scene is committed yet.

### Phase 1.1 — Repository and static validation
- [x] Confirmed GitHub repository, `main` base, active branch, working tree and all scaffold files. Original tracked README and Unity ignore template were preserved; the ignore file gained Python cache rules for the validation tool.
- [x] Reviewed all 20 C# scripts and six JSON files; checked package/version references, required metadata, input assumptions, save/transition logic and scene dependencies. A reproducible, read-only `Tools/validate_scaffold.py` records the core checks.
- [x] Corrected likely compilation defects (`ObjectiveListWrapper`, null-conditional assignments, bootstrap scene enum), bootstrap save path and camera tag, and the project version revision; updated the Unity 6 package declarations. None has been verified by Unity.
- [x] Added 50 unique, standard-format `.meta` files for current visible assets/folders. They were generated outside the Editor; their importers still need Unity verification.
- [x] Recorded unresolved scene/UI, data, input, save/transition and project settings issues in `Docs/Bug_Risk_Register.md` and added `Docs/Repository_Setup.md`.
- [ ] Resolve packages and import/compile in Unity 6000.3.0f1; validate generated settings/metadata and re-test source fixes.

### Phase C/D — Gameplay Prototype + Vertical Slice
- [ ] **Not started in Unity yet.** Scripts are written but uncompiled/unplayed. No scene has been opened in the Editor, no playtest has occurred, no build has been produced. This is stated explicitly per engineering standards — nothing below is claimed as tested.

---

## 2. What is NOT done / not claimed

- Unity 6.3 LTS is **not installed** in this sandbox (Debian 12, no GPU, 3.8 GB RAM). No Editor run, no compilation check, no playtest, no build.
- No rendering pipeline has been visually/performance tested or configured with pipeline assets. URP is a **recommendation** (T-01), not a validated choice.
- No art, audio, cinematics, character models, scene or build-scene list exist yet — only an unrun greybox generator. The UI, dialogue/objective assignments, evidence loading, progression and post-office access are incomplete (B-03–B-05).
- Legacy input settings, package lockfile, full Unity project settings, save/load and scene-transition behaviour remain unverified (B-06–B-12).
- The master PDF was reviewed but is not included in this checkout; distribute the source document separately if needed.
- Prakash's message text is a **clearly marked placeholder** (OPEN DECISION). The buried secret, deity, village name, and ending are **not implemented** as canon.

---

## 3. Next actions (in order)

1. Follow `Docs/Repository_Setup.md`: install **Unity 6000.3.0f1** and open the repository root in Unity Hub. Let the Editor resolve packages and import; inspect and record Console errors before proceeding.
2. Review generated package lock/settings and the checked-in `.meta` files. Set Active Input Handling to **Both** and import TMP Essential Resources; commit reviewed Unity-generated changes with their assets.
3. If compilation succeeds, optionally run **HOLLOW → Bootstrap Vertical Slice Scene**. The generator refuses to replace an existing scene; it does not make a playable slice. Record actual Editor results in the Bug & Risk Register.
4. After the validation gate, resolve the documented UI/data/geometry, input, saving and scene-lifetime blockers before attempting an end-to-end playtest or feature work.
5. **Approval needed** before content locks: Prakash message text, village name and first investigation lead destination (see Story Register).

---

## 4. How to update this report

After each milestone: move items from `[ ]` to `[x]`, add date + commit hash, and link the relevant register entry. Never mark a feature tested until it has been played in the Editor on the target machine.
