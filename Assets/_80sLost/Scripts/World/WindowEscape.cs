using UnityEngine;

namespace Lost80s
{
    [RequireComponent(typeof(Collider))]
    public class WindowEscape : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private AudioClip escapeClip;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;
            PlayerAudio.Play(escapeClip);
            // If a decoy door was disturbed, EndingManager swaps this for the fatal ending.
            EndingManager.TriggerEscape();
        }
    }
}
