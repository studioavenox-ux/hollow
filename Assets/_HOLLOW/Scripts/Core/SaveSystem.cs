using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Hollow.Story;

namespace Hollow.Core
{
    /// <summary>
    /// JSON checkpoint save system (Master §11.6).
    /// Saves story flags, collected evidence IDs, current objective,
    /// and player position to Application.persistentDataPath.
    /// Save only at clear story checkpoints — never mid-cinematic.
    /// Note: JsonUtility does not support Dictionary, so flags are
    /// stored as a serializable key/value list.
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        private const string SaveFileName = "hollow_save.json";

        [Serializable]
        private class FlagEntry
        {
            public string key;
            public bool value;
        }

        [Serializable]
        private class SaveData
        {
            public string savedAtUtc;
            public List<FlagEntry> storyFlags = new List<FlagEntry>();
            public List<string> collectedEvidenceIds = new List<string>();
            public string currentObjectiveId;
            public Vector3 playerPosition;
            public Vector3 playerRotation;
        }

        public string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        public void SaveGame()
        {
            try
            {
                var data = new SaveData
                {
                    savedAtUtc = DateTime.UtcNow.ToString("o"),
                    collectedEvidenceIds = EvidenceManager.CollectedIdsSnapshot(),
                    currentObjectiveId = ObjectiveManager.CurrentObjectiveIdSnapshot(),
                    playerPosition = transform.position,
                    playerRotation = transform.eulerAngles
                };

                foreach (var kvp in StoryFlags.Snapshot())
                {
                    data.storyFlags.Add(new FlagEntry { key = kvp.Key, value = kvp.Value });
                }

                // Prefer the player's actual transform if available.
                if (GameManager.Instance != null && GameManager.Instance.Player != null)
                {
                    var t = GameManager.Instance.Player.transform;
                    data.playerPosition = t.position;
                    data.playerRotation = t.eulerAngles;
                }

                string json = JsonUtility.ToJson(data, prettyPrint: true);
                File.WriteAllText(SavePath, json);
                Debug.Log($"[HOLLOW] Checkpoint saved to {SavePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[HOLLOW] Save failed: {ex.Message}");
            }
        }

        public bool HasSave() => File.Exists(SavePath);

        public bool LoadGame()
        {
            if (!HasSave())
            {
                Debug.LogWarning("[HOLLOW] No save file found.");
                return false;
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                var data = JsonUtility.FromJson<SaveData>(json);
                if (data == null) return false;

                var flags = new Dictionary<string, bool>();
                if (data.storyFlags != null)
                {
                    foreach (var entry in data.storyFlags)
                    {
                        if (entry != null && !string.IsNullOrEmpty(entry.key))
                        {
                            flags[entry.key] = entry.value;
                        }
                    }
                }
                StoryFlags.Restore(flags);
                EvidenceManager.RestoreCollectedIds(data.collectedEvidenceIds);
                ObjectiveManager.RestoreCurrentObjective(data.currentObjectiveId);

                if (GameManager.Instance != null && GameManager.Instance.Player != null)
                {
                    var t = GameManager.Instance.Player.transform;
                    t.position = data.playerPosition;
                    t.rotation = Quaternion.Euler(data.playerRotation);
                }

                Debug.Log($"[HOLLOW] Checkpoint loaded from {data.savedAtUtc}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[HOLLOW] Load failed: {ex.Message}");
                return false;
            }
        }

        public void DeleteSave()
        {
            if (HasSave()) File.Delete(SavePath);
        }
    }
}
