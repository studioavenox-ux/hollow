using UnityEngine;
using TMPro;
using Hollow.Story;

namespace Hollow.UI
{
    /// <summary>
    /// Minimal HUD: interaction prompt, crosshair, current objective reminder.
    /// Kept diegetic/minimal per Master §14.1 — clarity over decoration.
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        public static HUDManager Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private TextMeshProUGUI objectiveText;
        [SerializeField] private GameObject crosshair;
        [SerializeField] private GameObject pausePanel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            if (pausePanel != null) pausePanel.SetActive(false);
        }

        private void OnEnable()
        {
            ObjectiveManager.OnObjectiveChanged += HandleObjectiveChanged;
        }

        private void OnDisable()
        {
            ObjectiveManager.OnObjectiveChanged -= HandleObjectiveChanged;
        }

        private void HandleObjectiveChanged(ObjectiveData objective)
        {
            if (objectiveText != null && objective != null)
            {
                objectiveText.text = objective.title ?? "";
            }
        }

        /// <summary>Shows "[E] prompt" or hides when null/empty.</summary>
        public void SetInteractionPrompt(string prompt)
        {
            if (promptText == null) return;
            promptText.text = string.IsNullOrEmpty(prompt) ? "" : $"[E] {prompt}";
        }

        public void SetPaused(bool paused)
        {
            if (pausePanel != null) pausePanel.SetActive(paused);
        }
    }
}
