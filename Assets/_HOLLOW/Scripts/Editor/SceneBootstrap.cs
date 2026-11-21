using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Hollow.Core;
using Hollow.Player;
using Hollow.Interaction;
using Hollow.Dialogue;
using Hollow.Story;
using Hollow.UI;

namespace Hollow.Editor
{
    /// <summary>
    /// Editor tool: generates the greybox VerticalSlice scene
    /// (arrival road → village entrance → post office).
    /// Run via menu: HOLLOW → Bootstrap Vertical Slice Scene.
    /// Generates a scene in the Editor instead of hand-authoring its
    /// serialized Unity YAML; the generator is version-controlled as C#.
    /// </summary>
    public static class SceneBootstrap
    {
        private const string ScenePath = "Assets/_HOLLOW/Scenes/VerticalSlice.unity";

        [MenuItem("HOLLOW/Bootstrap Vertical Slice Scene")]
        public static void Bootstrap()
        {
            // Never replace a previously generated or hand-authored scene.
            if (File.Exists(ScenePath) || AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null)
            {
                EditorUtility.DisplayDialog("HOLLOW", $"A scene already exists at {ScenePath}. It was not overwritten.", "OK");
                return;
            }

            // A single-scene bootstrap closes the current scene; allow the user to save or cancel.
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // --- Lighting / atmosphere placeholders ---
            var sun = new GameObject("Directional Light (Overcast)");
            var light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 0.7f;
            light.color = new Color(0.85f, 0.88f, 0.92f);
            sun.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.72f, 0.76f, 0.78f);
            RenderSettings.fogDensity = 0.035f;
            RenderSettings.ambientLight = new Color(0.55f, 0.58f, 0.6f);

            // --- Ground ---
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground (Greybox)";
            ground.transform.localScale = new Vector3(12f, 1f, 12f);

            // --- Mountain road (arrival) ---
            var road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.name = "Mountain Road (Arrival)";
            road.transform.position = new Vector3(0f, 0.05f, -18f);
            road.transform.localScale = new Vector3(4f, 0.1f, 22f);

            // --- Village entrance (simple houses) ---
            for (int i = 0; i < 4; i++)
            {
                var house = GameObject.CreatePrimitive(PrimitiveType.Cube);
                house.name = $"Village House {i + 1} (Greybox)";
                float x = (i % 2 == 0) ? -6f : 6f;
                float z = -2f + (i / 2) * 6f;
                house.transform.position = new Vector3(x, 1.5f, z);
                house.transform.localScale = new Vector3(4f, 3f, 4f);
            }

            // --- Post office building ---
            var postOffice = GameObject.CreatePrimitive(PrimitiveType.Cube);
            postOffice.name = "Post Office (Greybox)";
            postOffice.transform.position = new Vector3(0f, 1.5f, 8f);
            postOffice.transform.localScale = new Vector3(8f, 3f, 6f);

            // Counter inside post office.
            var counter = GameObject.CreatePrimitive(PrimitiveType.Cube);
            counter.name = "Post Office Counter";
            counter.transform.position = new Vector3(0f, 0.6f, 7f);
            counter.transform.localScale = new Vector3(5f, 1.2f, 0.8f);

            // --- Player ---
            var player = new GameObject("Player");
            player.tag = "Player";
            player.transform.position = new Vector3(0f, 1f, -24f);
            var cc = player.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.35f;
            player.AddComponent<FirstPersonController>();
            // Note: cc is configured above; kept as a named local for clarity.

            var camGo = new GameObject("Player Camera");
            camGo.tag = "MainCamera"; // InteractionRaycaster resolves Camera.main at runtime.
            camGo.transform.SetParent(player.transform, false);
            camGo.transform.localPosition = new Vector3(0f, 0.6f, 0f);
            var cam = camGo.AddComponent<Camera>();
            cam.fieldOfView = 70f;
            camGo.AddComponent<AudioListener>();

            // --- Interaction: Gopinath (dialogue trigger) ---
            var gopinath = new GameObject("Gopinath Sahu (NPC Placeholder)");
            gopinath.transform.position = new Vector3(0f, 0f, 6f);
            var gopinathBody = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            gopinathBody.name = "Gopinath Body";
            gopinathBody.transform.SetParent(gopinath.transform, false);
            gopinathBody.transform.localPosition = new Vector3(0f, 0.9f, 0f);
            gopinath.AddComponent<GopinathDialogueTrigger>();

            // --- Interaction: Prakash package (inspectable) ---
            var package = new GameObject("Prakash Package (Inspectable)");
            package.transform.position = new Vector3(0f, 1.3f, 7f);
            var packageMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            packageMesh.name = "Package Mesh";
            packageMesh.transform.SetParent(package.transform, false);
            packageMesh.transform.localScale = new Vector3(0.4f, 0.25f, 0.3f);
            package.AddComponent<InspectableObject>();

            // --- Core systems ---
            var gameManagerGo = new GameObject("GameManager");
            gameManagerGo.AddComponent<GameManager>();
            gameManagerGo.AddComponent<SaveSystem>();
            gameManagerGo.AddComponent<SceneTransitionManager>();

            var dialogueGo = new GameObject("DialogueManager");
            dialogueGo.AddComponent<DialogueManager>();

            var objectiveGo = new GameObject("ObjectiveManager");
            objectiveGo.AddComponent<ObjectiveManager>();

            var evidenceGo = new GameObject("EvidenceManager");
            evidenceGo.AddComponent<EvidenceManager>();

            var journalGo = new GameObject("JournalManager");
            journalGo.AddComponent<JournalManager>();

            var interactionGo = new GameObject("InteractionRaycaster");
            interactionGo.AddComponent<InteractionRaycaster>();

            // --- Minimal UI canvas (HUD + subtitles + journal + evidence) ---
            BuildMinimalUI();

            if (!EditorSceneManager.SaveScene(scene, ScenePath))
            {
                Debug.LogError($"[HOLLOW] Failed to save the greybox scene at {ScenePath}.");
                EditorUtility.DisplayDialog("HOLLOW", $"Could not save the scene to {ScenePath}. Check the Console.", "OK");
                return;
            }
            AssetDatabase.Refresh();

            Debug.Log($"[HOLLOW] VerticalSlice greybox scene generated at {ScenePath}.");
            EditorUtility.DisplayDialog("HOLLOW",
                $"VerticalSlice greybox scaffold generated.\n\n{ScenePath}\n\nThe scene is incomplete and untested; see Docs/Bug_Risk_Register.md for missing wiring and known issues.",
                "OK");
        }

        private static void BuildMinimalUI()
        {
            var canvasGo = new GameObject("Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // HUD
            var hudGo = new GameObject("HUDManager");
            hudGo.transform.SetParent(canvasGo.transform, false);
            hudGo.AddComponent<HUDManager>();

            // Subtitles
            var subtitleGo = new GameObject("SubtitleDisplay");
            subtitleGo.transform.SetParent(canvasGo.transform, false);
            subtitleGo.AddComponent<SubtitleDisplay>();

            // Journal + evidence viewers
            var journalGo = new GameObject("JournalUI");
            journalGo.transform.SetParent(canvasGo.transform, false);
            journalGo.AddComponent<JournalUI>();

            var evidenceGo = new GameObject("EvidenceViewerUI");
            evidenceGo.transform.SetParent(canvasGo.transform, false);
            evidenceGo.AddComponent<EvidenceViewerUI>();

            var pauseGo = new GameObject("PauseMenu");
            pauseGo.transform.SetParent(canvasGo.transform, false);
            pauseGo.AddComponent<PauseMenu>();
        }
    }

}
