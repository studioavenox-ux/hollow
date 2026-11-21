using System.Collections;
using UnityEngine;
using Hollow.Core;
using Hollow.Story;

namespace Hollow.Dialogue
{
    /// <summary>
    /// Plays data-driven conversations with subtitles.
    /// Every spoken story line is subtitled (Master §7/§13.2).
    /// Locks player controls during dialogue; advances on E / click.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [SerializeField] private SubtitleDisplay subtitleDisplay;
        [SerializeField] private float lineDisplayTime = 4f;

        private bool _isPlaying;
        private Coroutine _playRoutine;

        public bool IsPlaying => _isPlaying;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            if (subtitleDisplay == null) subtitleDisplay = FindFirstObjectByType<SubtitleDisplay>();
        }

        /// <summary>Starts a conversation loaded from JSON data.</summary>
        public void PlayConversation(DialogueConversationData conversation)
        {
            if (_isPlaying || conversation == null || conversation.lines == null) return;
            _playRoutine = StartCoroutine(PlayRoutine(conversation));
        }

        /// <summary>Convenience: load from a TextAsset and play.</summary>
        public void PlayFromAsset(TextAsset jsonAsset)
        {
            var data = DialogueLoader.Load(jsonAsset);
            PlayConversation(data);
        }

        private IEnumerator PlayRoutine(DialogueConversationData conversation)
        {
            _isPlaying = true;
            if (GameManager.Instance != null && GameManager.Instance.Player != null)
            {
                GameManager.Instance.Player.ControlsEnabled = false;
            }

            foreach (var line in conversation.lines)
            {
                if (line == null || string.IsNullOrEmpty(line.text)) continue;
                subtitleDisplay?.Show(line.speaker, line.text);

                // Wait for player advance OR timeout (whichever first).
                float elapsed = 0f;
                bool advanced = false;
                while (elapsed < lineDisplayTime && !advanced)
                {
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                    {
                        advanced = true;
                        break;
                    }
                    elapsed += Time.unscaledDeltaTime;
                    yield return null;
                }
            }

            subtitleDisplay?.Hide();
            _isPlaying = false;
            if (GameManager.Instance != null && GameManager.Instance.Player != null)
            {
                GameManager.Instance.Player.ControlsEnabled = true;
            }

            if (!string.IsNullOrEmpty(conversation.onCompleteFlag))
            {
                StoryFlags.Set(conversation.onCompleteFlag, true);
            }

            if (!string.IsNullOrEmpty(conversation.nextObjectiveId))
            {
                ObjectiveManager.SetCurrentObjective(conversation.nextObjectiveId);
            }

            GameManager.Instance?.SaveCheckpoint();
            _playRoutine = null;
        }

        public void StopConversation()
        {
            if (_playRoutine != null) StopCoroutine(_playRoutine);
            _playRoutine = null;
            _isPlaying = false;
            subtitleDisplay?.Hide();
            if (GameManager.Instance != null && GameManager.Instance.Player != null)
            {
                GameManager.Instance.Player.ControlsEnabled = true;
            }
        }
    }
}
