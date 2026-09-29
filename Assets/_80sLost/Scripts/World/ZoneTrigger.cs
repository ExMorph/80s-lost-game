using UnityEngine;

namespace Lost80s
{
    /// <summary>
    /// Marks the entrance to a zone: turns a set of GameObjects on and another
    /// set off (lights, ambience props, whatever the zone owns), and plays the
    /// screen blink/grain-spike transition. Drop one at each zone boundary.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ZoneTrigger : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private bool triggerOnce = true;

        [Header("Objects to toggle")]
        [SerializeField] private GameObject[] activateOnEnter;
        [SerializeField] private GameObject[] deactivateOnEnter;

        [Header("Screen effect")]
        [SerializeField] private float grainBoost = 0.6f;
        [SerializeField] private float effectDuration = 1f;

        [Header("Sound")]
        [SerializeField] private AudioClip enterClip;

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

            foreach (GameObject go in activateOnEnter)
            {
                if (go != null) go.SetActive(true);
            }
            foreach (GameObject go in deactivateOnEnter)
            {
                if (go != null) go.SetActive(false);
            }

            ZoneEffects.Pulse(grainBoost, effectDuration);
            PlayerAudio.Play(enterClip);
        }
    }
}
