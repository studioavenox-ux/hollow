using UnityEngine;
using Hollow.Player;
using Hollow.Interaction;
using Hollow.Dialogue;
using Hollow.Story;
using Hollow.UI;

namespace Hollow.Core
{
    /// <summary>
    /// Bootstraps and holds references to the separated game systems.
    /// Deliberately thin — it does NOT contain gameplay logic itself
    /// (Master §15.3: avoid one oversized manager).
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Systems (assigned in Inspector or by SceneBootstrap)")]
        [SerializeField] private FirstPersonController player;
        [SerializeField] private InteractionRaycaster interactionRaycaster;
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private ObjectiveManager objectiveManager;
        [SerializeField] private EvidenceManager evidenceManager;
        [SerializeField] private JournalManager journalManager;
        [SerializeField] private HUDManager hudManager;
        [SerializeField] private SaveSystem saveSystem;
        [SerializeField] private SceneTransitionManager sceneTransitionManager;

        public FirstPersonController Player => player;
        public InteractionRaycaster InteractionRaycaster => interactionRaycaster;
        public DialogueManager DialogueManager => dialogueManager;
        public ObjectiveManager ObjectiveManager => objectiveManager;
        public EvidenceManager EvidenceManager => evidenceManager;
        public JournalManager JournalManager => journalManager;
        public HUDManager HudManager => hudManager;
        public SaveSystem SaveSystem => saveSystem;
        public SceneTransitionManager SceneTransitionManager => sceneTransitionManager;

        private bool _isPaused;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Auto-wire missing references from children/siblings if left unassigned.
            if (player == null) player = FindFirstObjectByType<FirstPersonController>();
            if (interactionRaycaster == null) interactionRaycaster = FindFirstObjectByType<InteractionRaycaster>();
            if (dialogueManager == null) dialogueManager = FindFirstObjectByType<DialogueManager>();
            if (objectiveManager == null) objectiveManager = FindFirstObjectByType<ObjectiveManager>();
            if (evidenceManager == null) evidenceManager = FindFirstObjectByType<EvidenceManager>();
            if (journalManager == null) journalManager = FindFirstObjectByType<JournalManager>();
            if (hudManager == null) hudManager = FindFirstObjectByType<HUDManager>();
            if (saveSystem == null) saveSystem = FindFirstObjectByType<SaveSystem>();
            if (sceneTransitionManager == null) sceneTransitionManager = FindFirstObjectByType<SceneTransitionManager>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0f : 1f;
            Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = _isPaused;
            hudManager?.SetPaused(_isPaused);
        }

        public bool IsPaused => _isPaused;

        /// <summary>Called at story checkpoints (Master §11.6). Never mid-cinematic.</summary>
        public void SaveCheckpoint()
        {
            saveSystem?.SaveGame();
        }
    }
}
