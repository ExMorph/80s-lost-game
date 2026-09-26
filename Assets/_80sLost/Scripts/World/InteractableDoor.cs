using UnityEngine;

namespace Lost80s
{
    [RequireComponent(typeof(Collider))]
    public class InteractableDoor : MonoBehaviour, IInteractable
    {
        [SerializeField] private float openAngle = 90f;
        [SerializeField] private float openSpeed = 3f;
        [SerializeField] private string closedPrompt = "Open";
        [SerializeField] private string openPrompt = "Close";
        [SerializeField] private bool startsLocked;
        [SerializeField] private string lockedPrompt = "Locked";

        [Header("Sound")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip openClip;
        [SerializeField] private AudioClip closeClip;
        [SerializeField] private AudioClip lockedClip;

        public string InteractPrompt => IsLocked ? lockedPrompt : (_isOpen ? openPrompt : closedPrompt);
        public bool IsLocked { get; private set; }

        private bool _isOpen;
        private Quaternion _closedRotation;
        private Quaternion _targetRotation;

        private void Awake()
        {
            IsLocked = startsLocked;
            _closedRotation = transform.localRotation;
            _targetRotation = _closedRotation;
        }

        private void Update()
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, _targetRotation, Time.deltaTime * openSpeed);
        }

        public void Interact(GameObject interactor)
        {
            if (IsLocked)
            {
                PlayClip(lockedClip);
                return;
            }
            _isOpen = !_isOpen;
            _targetRotation = _isOpen ? _closedRotation * Quaternion.Euler(0f, openAngle, 0f) : _closedRotation;
            PlayClip(_isOpen ? openClip : closeClip);
        }

        public void Unlock()
        {
            IsLocked = false;
            Interact(null);
        }

        private void PlayClip(AudioClip clip)
        {
            if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
        }
    }
}
