using UnityEngine;
using Hollow.Dialogue;

namespace Hollow.Interaction
{
    /// <summary>
    /// Dialogue trigger for Gopinath Sahu. Assign the dialogue JSON
    /// (Assets/_HOLLOW/Data/Dialogue/Gopinath_FirstMeeting.json) in the
    /// Inspector; plays the conversation on interact.
    /// Runtime component (not Editor-only) so it ships in builds.
    /// </summary>
    public class GopinathDialogueTrigger : Interactable
    {
        [SerializeField] private TextAsset dialogueJson;

        private void Awake()
        {
            SetPrompt("Talk to Gopinath Sahu");
        }

        public override void Interact()
        {
            base.Interact();
            if (dialogueJson != null)
            {
                DialogueManager.Instance?.PlayFromAsset(dialogueJson);
            }
            else
            {
                Debug.LogWarning("[HOLLOW] GopinathDialogueTrigger: no dialogue JSON assigned. Assign Gopinath_FirstMeeting.json in the Inspector.");
            }
        }
    }
}
