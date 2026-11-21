using UnityEngine;
using Hollow.Dialogue;
using Hollow.UI;

namespace Hollow.Interaction
{
    /// <summary>
    /// Center-screen raycast that finds the nearest <see cref="Interactable"/>
    /// in range, shows its prompt via HUD, and triggers it on E.
    /// </summary>
    public class InteractionRaycaster : MonoBehaviour
    {
        [SerializeField] private Camera interactionCamera;
        [SerializeField] private float maxDistance = 3f;
        [SerializeField] private LayerMask interactableMask = ~0;

        private Interactable _currentTarget;

        public Interactable CurrentTarget => _currentTarget;

        private void Awake()
        {
            if (interactionCamera == null) interactionCamera = Camera.main;
        }

        private void Update()
        {
            if (interactionCamera == null) return;

            // Do not interact while dialogue or evidence inspection is open.
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsPlaying) return;
            if (EvidenceViewerUI.Instance != null && EvidenceViewerUI.Instance.IsOpen) return;

            UpdateTarget();

            if (_currentTarget != null && Input.GetKeyDown(KeyCode.E))
            {
                _currentTarget.Interact();
            }
        }

        private void UpdateTarget()
        {
            _currentTarget = null;

            Ray ray = new Ray(interactionCamera.transform.position, interactionCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactableMask))
            {
                var interactable = hit.collider.GetComponentInParent<Interactable>();
                if (interactable != null && interactable.IsEnabled)
                {
                    _currentTarget = interactable;
                }
            }

            HUDManager.Instance?.SetInteractionPrompt(
                _currentTarget != null ? _currentTarget.PromptText : null);
        }
    }
}
