using UnityEngine;
using UnityEngine.Events;

namespace Hollow.Interaction
{
    /// <summary>
    /// Base interactable. Attach to any object the player can use.
    /// Shows a prompt via HUD and fires <see cref="OnInteract"/> on E.
    /// Reserve interactions for meaningful discovery (Master §11.2).
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        [Header("Interaction")]
        [SerializeField] private string promptText = "Interact";
        [SerializeField] private float interactionRange = 2.5f;
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private UnityEvent onInteract;

        public string PromptText => promptText;
        public float InteractionRange => interactionRange;
        public bool IsEnabled => isEnabled;
        public UnityEvent OnInteract => onInteract;

        public void SetEnabled(bool enabled) => isEnabled = enabled;

        /// <summary>For subclasses to set the prompt at runtime.</summary>
        protected void SetPrompt(string prompt) => promptText = prompt;

        public virtual void Interact()
        {
            onInteract?.Invoke();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}
