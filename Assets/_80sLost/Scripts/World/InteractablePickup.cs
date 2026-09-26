using UnityEngine;
using UnityEngine.Events;

namespace Lost80s
{
    [RequireComponent(typeof(Collider))]
    public class InteractablePickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private string prompt = "Pick up";
        public UnityEvent onPickedUp;

        public string InteractPrompt => prompt;

        public void Interact(GameObject interactor)
        {
            onPickedUp?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
