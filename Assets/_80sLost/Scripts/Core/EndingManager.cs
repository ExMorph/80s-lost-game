using System.Collections;
using UnityEngine;

namespace Lost80s
{
    /// <summary>
    /// Tracks how long the level has been running and decides which ending plays
    /// when the player escapes. Place one in the scene to expose the sound
    /// fields in the Inspector - if none exists, it auto-creates a bare one so
    /// the timer still works.
    /// </summary>
    public class EndingManager : MonoBehaviour
    {
        private static EndingManager _instance;

        [SerializeField] private float targetSeconds = 180f;

        [Header("Sound")]
        [SerializeField] private AudioClip winClip;
        [SerializeField] private AudioClip loseClip;
        [Tooltip("Played over the final 'Субъект пропал без вести' line.")]
        [SerializeField] private AudioClip missingClip;
        [Tooltip("Played over the 'Испытуемый упал' line when the anomaly was disturbed before escaping.")]
        [SerializeField] private AudioClip fellClip;

        private float _startTime;
        private bool _ended;
        private bool _disturbed;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            _startTime = Time.time;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            EnsureInstance();
        }

        public static void TriggerEscape()
        {
            EnsureInstance();
            _instance.HandleEscape();
        }

        /// <summary>Immediately ends the run in failure (caught by the anomaly).</summary>
        public static void TriggerCaught()
        {
            EnsureInstance();
            _instance.HandleCaught();
        }

        /// <summary>Marks the run as tainted - knocking on a decoy door disturbs the anomaly
        /// so the "correct" window no longer saves you.</summary>
        public static void MarkDisturbed()
        {
            EnsureInstance();
            _instance._disturbed = true;
        }

        private static void EnsureInstance()
        {
            if (_instance != null) return;
            var go = new GameObject("EndingManager");
            _instance = go.AddComponent<EndingManager>();
        }

        private void HandleEscape()
        {
            if (_ended) return;
            _ended = true;

            if (_disturbed)
            {
                PlayClip(fellClip);
                HintUI.Show("Испытуемый упал...", 60f);
                return;
            }

            float elapsed = Time.time - _startTime;
            if (elapsed <= targetSeconds)
            {
                PlayClip(winClip);
                HintUI.Show("Поздравляем! Вы выбрались вовремя.", 15f);
            }
            else
            {
                TriggerLossSequence();
            }
        }

        private void HandleCaught()
        {
            if (_ended) return;
            _ended = true;
            TriggerLossSequence();
        }

        private void TriggerLossSequence()
        {
            PlayClip(loseClip);
            StartCoroutine(LateEndingSequence());
        }

        private IEnumerator LateEndingSequence()
        {
            HintUI.Show("Вас не спасти - прощайте", 4f);
            yield return new WaitForSeconds(4f);
            HintUI.Show("Субъект пропал без вести...", 60f);
            PlayClip(missingClip);
        }

        private void PlayClip(AudioClip clip)
        {
            PlayerAudio.Play(clip);
        }
    }
}
