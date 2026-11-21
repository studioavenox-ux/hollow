using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hollow.Story
{
    /// <summary>
    /// Journal data layer: current objective + collected evidence IDs.
    /// UI reads from here (Master §11.3 working proposal: people / places /
    /// documents / order clues grouping can be added once the loop is proven).
    /// </summary>
    public class JournalManager : MonoBehaviour
    {
        public static JournalManager Instance { get; private set; }

        private readonly List<string> _discoveries = new List<string>();

        public static event Action OnJournalUpdated;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            EvidenceManager.OnEvidenceCollected += HandleEvidenceCollected;
            ObjectiveManager.OnObjectiveChanged += HandleObjectiveChanged;
        }

        private void OnDisable()
        {
            EvidenceManager.OnEvidenceCollected -= HandleEvidenceCollected;
            ObjectiveManager.OnObjectiveChanged -= HandleObjectiveChanged;
        }

        private void HandleEvidenceCollected(string evidenceId)
        {
            if (!_discoveries.Contains(evidenceId))
            {
                _discoveries.Add(evidenceId);
                OnJournalUpdated?.Invoke();
            }
        }

        private void HandleObjectiveChanged(ObjectiveData objective)
        {
            OnJournalUpdated?.Invoke();
        }

        public IReadOnlyList<string> Discoveries => _discoveries;
    }
}
