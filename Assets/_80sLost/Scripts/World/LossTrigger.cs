using UnityEngine;

namespace Lost80s
{
    /// <summary>
    /// Generic "entering this trigger ends the run" volume - e.g. getting caught
    /// by the corridor figure once the anomaly has been disturbed.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class LossTrigger : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private bool triggerOnce = true;

        private bool _used;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggerOnce && _used) return;
            if (!other.CompareTag(playerTag)) return;
            _used = true;
            EndingManager.TriggerCaught();
        }
    }
}
