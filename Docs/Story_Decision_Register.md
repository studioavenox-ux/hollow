# HOLLOW — Story Decision Register

Source of truth: Master Development Document v1.0 (21 Nov 2026).

Rule: **APPROVED CANON** may be treated as fact, although much of it is not implemented in the current scaffold. **WORKING PROPOSAL** is draft direction. **OPEN DECISION** is NOT implemented as final story content. Nothing here silently changes canon.

## APPROVED CANON (reference; implementation status below)

| ID | Fact | Where it appears in scaffold |
|---|---|---|
| C-01 | Arjun Nayak, 24, left village at 12, returns after father's death; loves Ananya Das | Return and father's death are framed in draft dialogue/objectives; age and relationship are not implemented. |
| C-02 | Ananya Das, 23, childhood friend, unconfessed feelings, dies mid-story; reveals Meera alive + connected to secrets | Not yet represented in slice JSON or a scene. |
| C-03 | Prakash Nayak: inherited order leadership (did not found it); took Arjun away at 12; lived elsewhere; secretly returned to gather evidence against Meera; killed by Meera shortly before present day | Name, death and package appear in draft JSON; the rest of the backstory is not implemented. |
| C-04 | Meera Nayak: alive, took control after Prakash's death, turned order into wealth/influence/control, richest in village, feared but outwardly respected, lives in forest-surrounded royal-style palace | Not yet represented in slice JSON; palace not built. |
| C-05 | Gopinath Sahu: postmaster, cold/suspicious/resentful toward Arjun, informs Arjun of Prakash's death, delivers package (photograph + message) | `Gopinath_FirstMeeting.json` is a draft; dialogue and package are not yet wired into the generated scene. |
| C-06 | The Order: generations old, Nayak bloodline leadership, NOT originally money-making; Meera transformed it. Fictional deity + buried secret beneath village (premise approved, content open) | Not yet represented in slice JSON; no mythology invented. |
| C-07 | Setting inspired by Daringbadi/Kandhamal, Eastern Ghats; fictional village; Odia environmental text selective; real Odia religion/culture must NOT be portrayed as evil | Bootstrap uses neutral greybox objects; no village name or Odia text is set (see S-04). |

## OPEN DECISIONS (must NOT be treated as canon)

| ID | Question | Scaffold handling | Needed before |
|---|---|---|---|
| S-01 | What exactly is buried beneath the village + Nayak connection? | **Not invented.** No buried-secret content, symbols, or rituals in code/data. | Act IV / finale production |
| S-02 | What did Prakash discover / what evidence did he gather? | Package message is a **clearly marked PLACEHOLDER** (`Prakash_Message_PLACEHOLDER.json`). Photograph is generic. | Vertical-slice content lock |
| S-03 | What does Meera ultimately want from the buried secret? | Not stated anywhere in scaffold. | Act III/IV |
| S-04 | Fictional village name? (KESARPUR was suggested, **not approved**) | Bootstrap uses unnamed greybox objects — KESARPUR is **not** used as canon. | Any public-facing text / signage |
| S-05 | Fictional deity: real / imagined / manipulated / ambiguous? + symbols, hierarchy, rules | No deity named or depicted. | Mythology design |
| S-06 | Villager mix: believers vs. fearful vs. opportunistic? | Only an unwired Gopinath placeholder/draft exists. Supporting cast (Sabitri, Rakesh, Dr. Nandita, Keeper) are **WORKING PROPOSALS** per Master §7.6 — not built. | Village population design |
| S-07 | Final confrontation + ending (branching vs. single)? | No ending content. | Finale production |
| S-08 | Prakash's personal responsibility as leader; Arjun's pre-12 memories of the order; where Prakash lived during separation | Left open; dialogue avoids asserting these. | Story spine lock |

## WORKING PROPOSALS (draft, not locked)

- P-01: Vertical slice = opening return-to-village sequence (Master §17). Implemented as the scaffold's target.
- P-02: Journal groups evidence into people/places/documents/order clues (§11.3). Grouping is not implemented; `JournalManager` only tracks collected IDs and the UI is unwired.
- P-03: Environmental/document puzzles emerge naturally (§11.5). No puzzles built yet — deferred until loop is proven.

## Change control

When Adi approves an OPEN item: update this register, the Master timeline/character sections it affects, and the change log below. Do not leave contradictory versions in separate docs (Master §20.1).
