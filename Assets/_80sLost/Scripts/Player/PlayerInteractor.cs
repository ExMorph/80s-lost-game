using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Lost80s
{
    /// <summary>
    /// Raycasts from the camera each frame looking for an IInteractable. Press E
    /// (or the interact key) to trigger it. Drop on the player's camera.
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private float range = 2.5f;
        [SerializeField] private LayerMask interactableMask = ~0;

        public event Action<string> PromptChanged;

        private IInteractable _current;

        private void Update()
        {
            IInteractable found = Raycast();

            if (found != _current)
            {
                _current = found;
                PromptChanged?.Invoke(_current?.InteractPrompt);
            }

            if (_current != null && ReadInteractPressed())
            {
                _current.Interact(gameObject);
            }
        }

        private IInteractable Raycast()
        {
            var ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, range, interactableMask, QueryTriggerInteraction.Ignore))
            {
                return hit.collider.GetComponentInParent<IInteractable>();
            }
            return null;
        }

        private bool ReadInteractPressed()
        {
#if ENABLE_INPUT_SYSTEM
            return Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.E);
#endif
        }
    }
}
