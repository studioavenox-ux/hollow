using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hollow.Dialogue
{
    /// <summary>Data classes for JSON dialogue (Master §15.1: data-driven, not hard-coded).</summary>
    [Serializable]
    public class DialogueLineData
    {
        public string speaker;
        public string text;
    }

    [Serializable]
    public class DialogueConversationData
    {
        public string id;
        public string title;
        public List<DialogueLineData> lines = new List<DialogueLineData>();
        public string onCompleteFlag;
        public string nextObjectiveId;
    }

    /// <summary>Static loader for dialogue JSON TextAssets.</summary>
    public static class DialogueLoader
    {
        public static DialogueConversationData Load(TextAsset jsonAsset)
        {
            if (jsonAsset == null)
            {
                Debug.LogError("[HOLLOW] Dialogue JSON asset is null.");
                return null;
            }
            return JsonUtility.FromJson<DialogueConversationData>(jsonAsset.text);
        }
    }
}
