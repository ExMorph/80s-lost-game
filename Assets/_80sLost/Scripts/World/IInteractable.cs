using UnityEngine;

namespace Lost80s
{
    public interface IInteractable
    {
        string InteractPrompt { get; }
        void Interact(GameObject interactor);
    }
}
