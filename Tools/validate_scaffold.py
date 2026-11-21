#!/usr/bin/env python3
"""Read-only HOLLOW scaffold checks. This is not a Unity compiler or playtest."""

import json
import re
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
ERRORS = []


def check(condition, message):
    if not condition:
        ERRORS.append(message)


def unique_object(pairs):
    result = {}
    for key, value in pairs:
        if key in result:
            raise ValueError("duplicate JSON key: " + key)
        result[key] = value
    return result


def invalid_constant(value):
    raise ValueError("invalid JSON constant: " + value)


def load_json(path):
    try:
        return json.loads(path.read_text(encoding="utf-8"), object_pairs_hook=unique_object,
                          parse_constant=invalid_constant)
    except (OSError, UnicodeError, ValueError) as exc:
        ERRORS.append(f"{path.relative_to(ROOT)}: {exc}")
        return None


def nonempty(value):
    return isinstance(value, str) and bool(value.strip())


def validate_json():
    manifest_path = ROOT / "Packages/manifest.json"
    files = sorted((ROOT / "Assets/_HOLLOW/Data").rglob("*.json"))
    files += sorted((ROOT / "Packages").glob("*.json"))
    check(manifest_path in files, "Missing Packages/manifest.json")
    values = {path: load_json(path) for path in files}
    manifest = values.get(manifest_path) or {}
    dependencies = manifest.get("dependencies", {}) if isinstance(manifest, dict) else {}
    if not isinstance(dependencies, dict):
        ERRORS.append("Package dependencies must be an object")
        dependencies = {}
    required = {
        "com.unity.inputsystem", "com.unity.render-pipelines.universal",
        "com.unity.test-framework", "com.unity.timeline", "com.unity.ugui",
        "com.unity.modules.jsonserialize", "com.unity.modules.physics", "com.unity.modules.ui",
    }
    check(isinstance(dependencies, dict) and required.issubset(dependencies),
          "Package manifest is missing required direct dependencies or modules")
    check("com.unity.textmeshpro" not in dependencies,
          "Standalone TextMeshPro conflicts with the Unity 6 uGUI package")
    for name, version in dependencies.items():
        check(isinstance(version, str) and bool(re.fullmatch(r"\d+\.\d+\.\d+(?:[-+][\w.-]+)?", version)),
              f"Invalid package version for {name}: {version!r}")

    base = ROOT / "Assets/_HOLLOW/Data"
    objectives = values.get(base / "Objectives/VerticalSlice_Objectives.json") or {}
    entries = objectives.get("objectives", []) if isinstance(objectives, dict) else []
    check(isinstance(entries, list) and len(entries) > 0, "Objectives list missing or empty")
    ids = []
    for item in entries if isinstance(entries, list) else []:
        check(isinstance(item, dict) and all(nonempty(item.get(k)) for k in ("id", "title", "description")),
              f"Malformed objective: {item!r}")
        if isinstance(item, dict) and nonempty(item.get("id")):
            ids.append(item["id"])
    check(len(ids) == len(set(ids)), "Duplicate objective IDs")
    check("vs_arrive" in ids, "Starting objective vs_arrive is missing")

    dialogue = values.get(base / "Dialogue/Gopinath_FirstMeeting.json") or {}
    if not isinstance(dialogue, dict):
        dialogue = {}
    lines = dialogue.get("lines", [])
    check(nonempty(dialogue.get("id")) and isinstance(lines, list) and bool(lines), "Dialogue ID or lines missing")
    for line in lines if isinstance(lines, list) else []:
        check(isinstance(line, dict) and nonempty(line.get("speaker")) and nonempty(line.get("text")),
              f"Malformed dialogue line: {line!r}")
    next_id = dialogue.get("nextObjectiveId")
    check(not next_id or next_id in ids, f"Dialogue nextObjectiveId is unknown: {next_id!r}")

    evidence_ids = []
    for name in ("Prakash_Message_PLACEHOLDER.json", "Prakash_Package.json", "Prakash_Photograph.json"):
        check(base / "Evidence" / name in values, f"Missing evidence data: {name}")
    for path in sorted((base / "Evidence").glob("*.json")):
        evidence = values.get(path) or {}
        check(isinstance(evidence, dict) and all(nonempty(evidence.get(k)) for k in ("id", "title", "bodyText"))
              and isinstance(evidence.get("isPlaceholder"), bool), f"Malformed evidence: {path.name}")
        if isinstance(evidence, dict) and nonempty(evidence.get("id")):
            evidence_ids.append(evidence["id"])
        if "PLACEHOLDER" in path.name.upper() or "Photograph" in path.name:
            check(isinstance(evidence, dict) and evidence.get("isPlaceholder") is True,
                  f"Expected a marked placeholder: {path.name}")
    check(len(evidence_ids) == len(set(evidence_ids)), "Duplicate evidence IDs")
    print(f"JSON: read {len(files)} files; checked package shape, objective IDs, dialogue links, and evidence data.")


def validate_scripts():
    scripts_root = ROOT / "Assets/_HOLLOW/Scripts"
    paths = sorted(scripts_root.rglob("*.cs"))
    expected = {
        "Core/GameManager.cs", "Core/SaveSystem.cs", "Core/SceneTransitionManager.cs", "Core/StoryFlags.cs",
        "Dialogue/DialogueData.cs", "Dialogue/DialogueManager.cs", "Dialogue/SubtitleDisplay.cs",
        "Editor/SceneBootstrap.cs", "Player/FirstPersonController.cs",
        "Interaction/GopinathDialogueTrigger.cs", "Interaction/InspectableObject.cs",
        "Interaction/Interactable.cs", "Interaction/InteractionRaycaster.cs",
        "Story/EvidenceManager.cs", "Story/JournalManager.cs", "Story/ObjectiveManager.cs",
        "UI/EvidenceViewerUI.cs", "UI/HUDManager.cs", "UI/JournalUI.cs", "UI/PauseMenu.cs",
    }
    missing = expected - {p.relative_to(scripts_root).as_posix() for p in paths}
    check(not missing, f"Missing scaffold scripts: {', '.join(sorted(missing))}")
    namespaces = set()
    declared = {}
    sources = {}
    for path in paths:
        text = path.read_text(encoding="utf-8")
        sources[path] = text
        match = re.search(r"(?m)^\s*namespace\s+(Hollow(?:\.\w+)+)", text)
        check(match is not None, f"No Hollow namespace in {path.relative_to(ROOT)}")
        namespace = match.group(1) if match else ""
        namespaces.add(namespace)
        for name in re.findall(r"\bclass\s+(\w+)", text):
            # This is a heuristic: the real Unity compiler resolves nested types and APIs.
            key = namespace + "." + name
            check(key not in declared, f"Possible duplicate class {key}: {declared.get(key)} and {path}")
            declared[key] = path
        for name in re.findall(r"\bpublic\s+class\s+(\w+)\s*:\s*(?:MonoBehaviour|Interactable)\b", text):
            check(path.stem == name, f"Attachable type {name} does not match {path.name}")
        check(not re.search(r"\?\.\s*\w+\s*=(?!=)", text),
              f"Possible null-conditional assignment in {path.relative_to(ROOT)}")
    for path, text in sources.items():
        for namespace in re.findall(r"(?m)^using\s+(Hollow(?:\.\w+)+);", text):
            check(namespace in namespaces, f"Unresolved internal namespace {namespace} in {path.name}")
    bootstrap = sources.get(ROOT / "Assets/_HOLLOW/Scripts/Editor/SceneBootstrap.cs", "")
    built_in = {"Light", "CharacterController", "Camera", "AudioListener", "Canvas"}
    known_types = {name.rsplit(".", 1)[-1] for name in declared}
    for name in re.findall(r"AddComponent<(\w+)>", bootstrap):
        check(name in built_in or name in known_types, f"Unknown bootstrap component {name}")
    check("NewSceneMode.Single" in bootstrap and "SaveScene(scene, ScenePath)" in bootstrap,
          "Bootstrap scene creation/save API does not match the expected scaffold")
    print(f"C#: inspected {len(paths)} files for namespaces, type names, and obvious reference issues (heuristic only).")


def validate_metadata():
    assets_root = ROOT / "Assets"
    assets = sorted(p for p in assets_root.rglob("*")
                    if not any(part.startswith(".") for part in p.relative_to(assets_root).parts)
                    and not p.name.endswith(".meta"))
    expected_meta = {Path(str(asset) + ".meta") for asset in assets}
    seen = {}
    for asset in assets:
        meta = Path(str(asset) + ".meta")
        if not meta.is_file():
            ERRORS.append(f"Missing metadata: {meta.relative_to(ROOT)}")
            continue
        text = meta.read_text(encoding="utf-8")
        match = re.search(r"(?m)^guid: ([0-9a-f]{32})$", text)
        check(text.startswith("fileFormatVersion: 2\n") and match is not None,
              f"Invalid metadata header/GUID: {meta.relative_to(ROOT)}")
        if asset.is_dir():
            importer = "DefaultImporter:"
        elif asset.suffix == ".cs":
            importer = "MonoImporter:"
        elif asset.suffix in (".json", ".md"):
            importer = "TextScriptImporter:"
        else:
            importer = None  # Other Unity asset types have their own importer formats.
        if importer:
            check(importer in text, f"Unexpected importer in {meta.relative_to(ROOT)}")
        if asset.is_dir():
            check("folderAsset: yes" in text, f"Missing folder marker in {meta.relative_to(ROOT)}")
        if match:
            guid = match.group(1)
            check(guid not in seen, f"Duplicate GUID in {meta.relative_to(ROOT)} and {seen.get(guid)}")
            seen[guid] = meta.relative_to(ROOT)
    for meta in assets_root.rglob("*.meta"):
        check(meta in expected_meta, f"Orphaned metadata: {meta.relative_to(ROOT)}")
    print(f"Metadata: checked {len(assets)} visible assets/folders, {len(seen)} GUIDs; Unity import still unverified.")


def validate_project_and_ignore():
    for relative in (
        "README.md", ".gitignore", "Assets/_HOLLOW/README.md", "Docs/Progress_Report.md",
        "Docs/Bug_Risk_Register.md", "Docs/Environment_Audit.md", "Docs/Technical_Decision_Register.md",
        "Docs/Story_Decision_Register.md", "Docs/Milestones.md", "Docs/Vertical_Slice_Plan.md",
        "Docs/Repository_Setup.md",
    ):
        check((ROOT / relative).is_file(), f"Missing required documentation: {relative}")
    version_path = ROOT / "ProjectSettings/ProjectVersion.txt"
    try:
        version = version_path.read_text(encoding="utf-8")
    except OSError as exc:
        ERRORS.append(str(exc))
        version = ""
    check("m_EditorVersion: 6000.3.0f1\n" in version and
          "m_EditorVersionWithRevision: 6000.3.0f1 (d1870ce95baf)" in version,
          "Unexpected Unity version/revision")
    try:
        for path in ("Library/test", "Temp/test", "Obj/test", "Build/test", "Builds/test",
                     "Logs/test", "UserSettings/test", "generated.csproj", "generated.sln"):
            result = subprocess.run(["git", "-C", str(ROOT), "check-ignore", "--no-index", "-q", path],
                                    stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, check=False)
            check(result.returncode == 0, f"Expected generated path to be ignored: {path}")
        result = subprocess.run(["git", "-C", str(ROOT), "check-ignore", "--no-index", "-q",
                                 "Assets/_HOLLOW/Scripts/Core/GameManager.cs.meta"],
                                stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, check=False)
        check(result.returncode == 1, "Asset .meta files should not be ignored")
    except FileNotFoundError:
        ERRORS.append("Git not available; cannot check .gitignore")
    print("Project: checked Unity version/revision and representative .gitignore paths.")


def main():
    validate_json()
    validate_scripts()
    validate_metadata()
    validate_project_and_ignore()
    if ERRORS:
        for error in ERRORS:
            print("ERROR:", error, file=sys.stderr)
        return 1
    print("PASS: read-only static checks. No Unity compilation, package resolution, scene import, or playtest was performed.")
    print("Known omissions and runtime risks: see Docs/Bug_Risk_Register.md.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
