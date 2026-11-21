using UnityEngine;
using TMPro;
using Hollow.Story;

namespace Hollow.UI
{
    /// <summary>
    /// Journal panel (Tab to toggle). Shows current objective and
    /// collected evidence IDs. Minimal working version for the slice;
    /// grouping/polish deferred until the loop is proven (Master §11.3).
    /// </summary>
    public class JournalUI : MonoBehaviour
    {
        public static JournalUI Instance { get; private set; }

        [SerializeField] private GameObject journalPanel;
        [SerializeField] private TextMeshProUGUI objectiveText;
        [SerializeField] private TextMeshProUGUI discoveriesText;

        private bool _isOpen;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            if (journalPanel != null) journalPanel.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            _isOpen = !_isOpen;
            if (journalPanel != null) journalPanel.SetActive(_isOpen);
            if (_isOpen) Refresh();
        }

        public void Refresh()
        {
            var current = ObjectiveManager.Instance?.CurrentObjective;
            if (objectiveText != null)
            {
                objectiveText.text = current != null
                    ? $"{current.title}\n{current.description}"
                    : "No active objective.";
            }

            if (discoveriesText != null)
            {
                var discoveries = JournalManager.Instance?.Discoveries;
                discoveriesText.text = (discoveries == null || discoveries.Count == 0)
                    ? "No discoveries yet."
                    : string.Join("\n", discoveries);
            }
        }
    }
}
