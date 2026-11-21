# HOLLOW — Vertical Slice Implementation Plan

**Slice:** Opening return-to-village sequence (Master §17, WORKING PROPOSAL).

**Goal:** Prove atmosphere, first-person interaction, character writing, evidence loop, and cinematic tone — without the full plot.

This is a plan, not a claim of implemented or tested behaviour. The current scaffold has no playable end-to-end slice; see `Docs/Bug_Risk_Register.md`.

## A. Arrival (mountain road)
- Greybox mountain-road approach: winding road, mist/fog, pine/forest silhouettes, distant valley. Player spawns at road start; short scripted walk-in or fade-in transition (in-engine, Timeline-ready).
- Systems: `FirstPersonController` (walk + look), `SceneTransitionManager` fade, ambient wind/rain audio placeholders, fog + cool lighting.
- Acceptance: player understands they have returned to the village region; controls feel reliable.

## B. Village Entrance
- Compact believable entrance: dirt road, traditional homes, market hints, school/service signage placeholders (Odia text deferred until verified — S-04), villagers' ambient presence (simple NPC placeholders, no full AI).
- Environmental storytelling: closed shutters, guarded glances (animation placeholders), everyday clutter.
- Systems: interaction raycast, HUD prompt, objective reminder.

## C. Post Office + Gopinath Sahu
- Playable post-office interior: counter, parcels, register, notice board.
- Gopinath interaction: cold, suspicious, resentful dialogue with subtitles (`DialogueManager` + `SubtitleDisplay`). He delivers the death news + package.
- Dialogue data: `Assets/_HOLLOW/Data/Dialogue/Gopinath_FirstMeeting.json` (draft, character-specific, no canon invented).

## D. Prakash's Package (inspection)
- Inspectable photograph + message (`InspectableObject` → `EvidenceViewerUI` → `EvidenceManager` → `JournalManager`).
- Message text = **PLACEHOLDER** pending S-02 approval. Photograph is generic (no invented story detail).
- Discovery recorded; journal entry created; checkpoint saved (`SaveSystem`).

## E. Next Objective
- Clear next objective displayed (`ObjectiveManager` + HUD + journal): follow the first investigation lead.
- Lead destination is **provisional** — needs Adi approval (Story Register S-02). Slice ends with objective set + save, demonstrating checkpoint/scene-transition behaviour.

## Out of scope for the slice (Master §17.3)
Full forest region, palace, complete mythology, supporting cast, branching dialogue, combat/crafting/stealth.

## Build order
1. Bootstrap greybox scene (Editor script) → 2. Player + interaction → 3. Dialogue/subtitles → 4. Inspection/evidence/journal → 5. Objectives/save/transition → 6. Lighting/audio polish → 7. Playtest + profile → 8. Fix + document.
