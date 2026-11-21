using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hollow.Story
{
    /// <summary>
    /// Tracks collected evidence IDs. Draft evidence content lives in
    /// Assets/_HOLLOW/Data/Evidence, but no runtime JSON loader is wired yet
    /// (see Docs/Bug_Risk_Register.md).
    /// </summary>
    public class EvidenceManager : MonoBehaviour
    {
        public static EvidenceManager Instance { get; private set; }

        private readonly HashSet<string> _collected = new HashSet<string>();

        public static event Action<string> OnEvidenceCollected;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public static void RegisterCollected(string evidenceId)
        {
            if (Instance == null || string.IsNullOrEmpty(evidenceId)) return;
            if (Instance._collected.Add(evidenceId))
            {
                OnEvidenceCollected?.Invoke(evidenceId);
                Debug.Log($"[HOLLOW] Evidence collected: {evidenceId}");
            }
        }

        public static bool HasCollected(string evidenceId)
        {
            return Instance != null && !string.IsNullOrEmpty(evidenceId) && Instance._collected.Contains(evidenceId);
        }

        public static List<string> CollectedIdsSnapshot()
        {
            if (Instance == null) return new List<string>();
            return new List<string>(Instance._collected);
        }

        public static void RestoreCollectedIds(List<string> ids)
        {
            if (Instance == null || ids == null) return;
            Instance._collected.Clear();
            foreach (var id in ids)
            {
                if (!string.IsNullOrEmpty(id)) Instance._collected.Add(id);
            }
        }
    }
}
