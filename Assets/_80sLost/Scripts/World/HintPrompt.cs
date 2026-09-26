using UnityEngine;

namespace Lost80s
{
    [RequireComponent(typeof(Collider))]
    public class HintPrompt : MonoBehaviour
    {
        [TextArea]
        [SerializeField] private string message;
        [SerializeField] private float displaySeconds = 6f;
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private bool triggerOnce = true;

        private bool _used;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;
            if (triggerOnce && _used) return;
            _used = true;

            HintUI.Show(message, displaySeconds);
            if (audioClip != null)
            {
                AudioSource.PlayClipAtPoint(audioClip, transform.position);
            }
        }
    }
}
