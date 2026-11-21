# HOLLOW — Environment Audit (21 Nov 2026)

Audited in the Arena sandbox (`/home/user/hollow`, branch `arena/895a51a9-hollow`).

| Area | Finding |
|---|---|
| OS | Debian GNU/Linux 12 (bookworm), kernel 6.1.158 x86_64 |
| CPU | 2× Intel Xeon @ 2.60 GHz (KVM virtualised) |
| RAM | 3.8 GB total, no swap |
| Disk | 21 GB total, ~20 GB free (4% used) |
| GPU | **None** — no `/dev/dri`, no VGA/3D device in `lspci` |
| Unity | **Not installed** — no `unity`/`Unity` binary, nothing under `/opt` or `/usr/local` |
| .NET / Mono | Not installed (`dotnet`, `mono` absent) |
| Node (for reference) | Present via `/usr/local/bin/node` (irrelevant to Unity) |
| Git | Repo `studioavenox-ux/hollow`, branch `arena/895a51a9-hollow` from `main` @ `0c34b7b`, working tree clean before this work |
| Existing project files | None — only `README.md` + Unity-standard `.gitignore`. Nothing to overwrite; no user data at risk |
| Master document | `HOLLOW_Master_Development_Document_v1.0.pdf` (24 pages) was received and reviewed; the PDF itself is not present in this repository snapshot |

## Implications

1. **Unity cannot be run here.** This sandbox has no GPU and 3.8 GB RAM — far below a viable Unity Editor + URP playtest machine. All C# in this change is **written but uncompiled**. It must be opened and compiled in Unity 6.3 LTS on the target PC before any "works" claim.
2. **No build can be produced here.** Do not treat this incomplete, unverified scaffold as a playable build. Unity import, compilation and package resolution have not occurred.
3. **Rendering pipeline cannot be tested here.** URP is recommended on paper (see Technical Register T-01), but no pipeline asset is configured and a visual/performance test on target hardware is still required.
4. **Static validation is possible:** source/JSON review, grammar parsing, metadata and Git checks do not substitute for an Editor import or playtest.

## Phase 1.1 validation environment and results

- GitHub confirmed `studioavenox-ux/hollow`, default branch `main`, working branch `arena/895a51a9-hollow`, based on `0c34b7b63aa3ad475f94b5ee61a4fee7ea038ad7`.
- `python3 Tools/validate_scaffold.py` checks six JSON files, objective/dialogue references, the 20 expected scripts using heuristics, 50 unique asset/folder metadata GUIDs, version format and representative `.gitignore` rules. Five deliberately invalid cases were detected using isolated temporary copies.
- A separate Tree-sitter C# grammar/declaration pass parsed all 20 files and 26 type declarations with no syntax-error nodes, duplicate qualified types, namespace-scope private types or null-conditional assignments. Neither check compiles C#, resolves Unity APIs/packages, imports scenes, tests behaviour or runs the Editor.
- Metadata was pre-generated using standard importer templates, not produced or validated by Unity. First import and the package lockfile/project settings remain outstanding.

## Target-machine checklist (for Adi / AVENOX Games)

- [ ] Install **Unity 6000.3.0f1** through Unity Hub; add Windows Build Support (IL2CPP) and Visual Studio/Rider if needed. The exact clone/open steps are in `Docs/Repository_Setup.md`.
- [ ] Record GPU + RAM and later profile the chosen rendering pipeline on target hardware (T-01).
- [ ] Open the repository root in Hub, allow package resolution/import, validate generated settings/metadata and record every Console error. Set Active Input Handling to **Both** and import TMP Essential Resources.
- [ ] Optionally run the greybox bootstrap after compilation, inspect the incomplete scene and log results; do not assume dialogue, evidence, objectives, transitions or saving work. Fix the recorded blockers before an end-to-end playtest.
