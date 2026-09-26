using UnityEngine;
using UnityEngine.Events;

namespace Lost80s
{
    [RequireComponent(typeof(Collider))]
    public class InteractablePickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private string prompt = "Pick up";
        [SerializeField] private AudioClip pickupClip;
        public UnityEvent onPickedUp;

        public string InteractPrompt => prompt;

        public void Interact(GameObject interactor)
        {
            onPickedUp?.Invoke();
            if (pickupClip != null) AudioSource.PlayClipAtPoint(pickupClip, transform.position);
            gameObject.SetActive(false);
        }
    }
}
