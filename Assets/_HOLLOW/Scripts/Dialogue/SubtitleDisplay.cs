using UnityEngine;
using TMPro;

namespace Hollow.Dialogue
{
    /// <summary>
    /// Subtitle display with adjustable size (Master §14.3 accessibility).
    /// Every spoken story line must pass through here (Master §7/§13.2).
    /// </summary>
    public class SubtitleDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI speakerText;
        [SerializeField] private TextMeshProUGUI bodyText;
        [SerializeField] private GameObject subtitlePanel;
        [SerializeField] private float defaultFontSize = 24f;

        private float _fontSizeMultiplier = 1f;

        private void Awake()
        {
            if (subtitlePanel != null) subtitlePanel.SetActive(false);
        }

        public void Show(string speaker, string text)
        {
            if (subtitlePanel != null) subtitlePanel.SetActive(true);
            if (speakerText != null) speakerText.text = string.IsNullOrEmpty(speaker) ? "" : speaker + ":";
            if (bodyText != null)
            {
                bodyText.text = text ?? "";
                bodyText.fontSize = defaultFontSize * _fontSizeMultiplier;
            }
        }

        public void Hide()
        {
            if (subtitlePanel != null) subtitlePanel.SetActive(false);
        }

        /// <summary>Accessibility: subtitle size 0.8×–1.6×.</summary>
        public void SetSizeMultiplier(float multiplier)
        {
            _fontSizeMultiplier = Mathf.Clamp(multiplier, 0.8f, 1.6f);
            if (bodyText != null)
            {
                bodyText.fontSize = defaultFontSize * _fontSizeMultiplier;
            }
        }
    }
}
