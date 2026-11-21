using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hollow.Story
{
    /// <summary>Objective data (JSON-driven, Master §15.1).</summary>
    [Serializable]
    public class ObjectiveData
    {
        public string id;
        public string title;
        public string description;
    }

    [Serializable]
    internal class ObjectiveListWrapper
    {
        public List<ObjectiveData> objectives = new List<ObjectiveData>();
    }

    /// <summary>
    /// Tracks the current investigation objective and notifies UI.
    /// Keeps the player oriented without revealing every clue location
    /// (Master §10.3).
    /// </summary>
    public class ObjectiveManager : MonoBehaviour
    {
        public static ObjectiveManager Instance { get; private set; }

        [SerializeField] private TextAsset objectivesJson;
        [SerializeField] private string startingObjectiveId = "vs_arrive";

        private readonly Dictionary<string, ObjectiveData> _objectives = new Dictionary<string, ObjectiveData>();
        private ObjectiveData _current;

        public static event Action<ObjectiveData> OnObjectiveChanged;

        public ObjectiveData CurrentObjective => _current;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            LoadObjectives();
        }

        private void Start()
        {
            if (_current == null && !string.IsNullOrEmpty(startingObjectiveId))
            {
                SetCurrentObjective(startingObjectiveId);
            }
        }

        private void LoadObjectives()
        {
            _objectives.Clear();
            if (objectivesJson == null)
            {
                Debug.LogWarning("[HOLLOW] ObjectiveManager: no objectives JSON assigned.");
                return;
            }
            var wrapper = JsonUtility.FromJson<ObjectiveListWrapper>(objectivesJson.text);
            if (wrapper?.objectives == null) return;
            foreach (var obj in wrapper.objectives)
            {
                if (obj != null && !string.IsNullOrEmpty(obj.id))
                {
                    _objectives[obj.id] = obj;
                }
            }
        }

        public static void SetCurrentObjective(string objectiveId)
        {
            if (Instance == null || string.IsNullOrEmpty(objectiveId)) return;
            if (Instance._objectives.TryGetValue(objectiveId, out var obj))
            {
                Instance._current = obj;
                OnObjectiveChanged?.Invoke(obj);
                Debug.Log($"[HOLLOW] Objective: {obj.title}");
            }
            else
            {
                Debug.LogWarning($"[HOLLOW] Objective not found: {objectiveId}");
            }
        }

        public static string CurrentObjectiveIdSnapshot()
        {
            return Instance?._current?.id;
        }

        public static void RestoreCurrentObjective(string objectiveId)
        {
            if (!string.IsNullOrEmpty(objectiveId)) SetCurrentObjective(objectiveId);
        }
    }
}
