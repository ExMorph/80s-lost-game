using UnityEngine;

namespace Lost80s
{
    /// <summary>
    /// Trips once, when the player reaches the trapped landing: enables the
    /// blocker geometry (sealing the shaft above and below) and shows the
    /// anomaly warning.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class StairwellSeal : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private GameObject[] blockers;
        [TextArea]
        [SerializeField] private string message = "Вы попали в аномалию. Немедленно выбирайтесь из здания.";
        [SerializeField] private float displaySeconds = 8f;
        [SerializeField] private AudioClip audioClip;
        [Tooltip("If true, reaching this trapped landing immediately ends the run instead of only sealing the exits.")]
        [SerializeField] private bool triggersLoss;

        private bool _triggered;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_triggered || !other.CompareTag(playerTag)) return;
            _triggered = true;

            foreach (GameObject blocker in blockers)
            {
                if (blocker != null) blocker.SetActive(true);
            }

            HintUI.Show(message, displaySeconds);
            PlayerAudio.Play(audioClip);
            if (triggersLoss) EndingManager.TriggerCaught();
        }
    }
}
