using UnityEngine;
using UnityEngine.Events;

namespace Lost80s
{
    [RequireComponent(typeof(Collider))]
    public class VHSTapePickup : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [TextArea]
        [SerializeField] private string loreText;
        [SerializeField] private AudioClip pickupClip;

        public UnityEvent<string> onCollected;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;

            onCollected?.Invoke(loreText);
            PlayerAudio.Play(pickupClip);
            gameObject.SetActive(false);
        }
    }
}
