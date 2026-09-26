using UnityEngine;

namespace Lost80s
{
    [RequireComponent(typeof(Collider))]
    public class WindowEscape : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;
            EndingManager.TriggerEscape();
        }
    }
}
