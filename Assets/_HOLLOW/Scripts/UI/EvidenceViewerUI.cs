using UnityEngine;
using TMPro;
using Hollow.Core;

namespace Hollow.UI
{
    /// <summary>
    /// Evidence inspection view. Shows title + body text; clearly marks
    /// placeholder content so unapproved story text is never mistaken
    /// for canon (Story Register S-02).
    /// </summary>
    public class EvidenceViewerUI : MonoBehaviour
    {
        public static EvidenceViewerUI Instance { get; private set; }

        [SerializeField] private GameObject viewerPanel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI bodyText;
        [SerializeField] private TextMeshProUGUI placeholderLabel;

        private bool _isOpen;

        public bool IsOpen => _isOpen;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            if (viewerPanel != null) viewerPanel.SetActive(false);
        }

        private void Update()
        {
            if (_isOpen && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)))
            {
                Close();
            }
        }

        public void Show(string title, string body, bool isPlaceholder)
        {
            if (viewerPanel != null) viewerPanel.SetActive(true);
            if (titleText != null) titleText.text = title ?? "Evidence";
            if (bodyText != null) bodyText.text = body ?? "";
            if (placeholderLabel != null)
            {
                placeholderLabel.gameObject.SetActive(isPlaceholder);
                if (isPlaceholder)
                {
                    placeholderLabel.text = "[PLACEHOLDER — narrative content not yet approved]";
                }
            }
            _isOpen = true;

            // Pause world while inspecting (but keep UI responsive).
            if (GameManager.Instance?.Player != null)
            {
                GameManager.Instance.Player.ControlsEnabled = false;
            }
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Close()
        {
            if (viewerPanel != null) viewerPanel.SetActive(false);
            _isOpen = false;
            if (GameManager.Instance?.Player != null)
            {
                GameManager.Instance.Player.ControlsEnabled = true;
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
