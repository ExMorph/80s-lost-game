using UnityEngine;
using UnityEngine.Events;

namespace Lost80s
{
    [RequireComponent(typeof(Collider))]
    public class ExitZone : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        public UnityEvent onPlayerExited;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;

            Debug.Log("Player reached the exit.");
            onPlayerExited?.Invoke();
        }
    }
}
