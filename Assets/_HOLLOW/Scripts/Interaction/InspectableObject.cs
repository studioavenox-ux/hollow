using UnityEngine;
using Hollow.Core;
using Hollow.UI;

namespace Hollow.Interaction
{
    /// <summary>
    /// Inspectable evidence object (photograph, message, document).
    /// Opens the evidence viewer, records the discovery, sets a story
    /// flag, and saves a checkpoint (Master §11.2/§11.3).
    /// </summary>
    public class InspectableObject : Interactable
    {
        [Header("Evidence")]
        [SerializeField] private string evidenceId;
        [SerializeField] private string evidenceTitle = "Evidence";
        [SerializeField] private string evidenceBody = "";
        [SerializeField] private string storyFlagOnInspect = "";
        [SerializeField] private bool isPlaceholder;

        public string EvidenceId => evidenceId;
        public string EvidenceTitle => evidenceTitle;
        public string EvidenceBody => evidenceBody;
        public bool IsPlaceholder => isPlaceholder;

        public override void Interact()
        {
            base.Interact();

            if (!string.IsNullOrEmpty(evidenceId))
            {
                Hollow.Story.EvidenceManager.RegisterCollected(evidenceId);
            }

            if (!string.IsNullOrEmpty(storyFlagOnInspect))
            {
                StoryFlags.Set(storyFlagOnInspect, true);
            }

            EvidenceViewerUI.Instance?.Show(evidenceTitle, evidenceBody, isPlaceholder);

            // Checkpoint after a meaningful discovery (Master §11.6).
            GameManager.Instance?.SaveCheckpoint();
        }
    }
}
