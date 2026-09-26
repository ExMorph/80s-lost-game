using System.Collections;
using UnityEngine;

namespace Lost80s
{
    /// <summary>
    /// Tracks how long the level has been running and decides which ending plays
    /// when the player escapes. Auto-creates itself on scene load so nothing has
    /// to be dragged into the scene.
    /// </summary>
    public class EndingManager : MonoBehaviour
    {
        private static EndingManager _instance;

        [SerializeField] private float targetSeconds = 180f;

        private float _startTime;
        private bool _ended;

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

        private static void EnsureInstance()
        {
            if (_instance != null) return;
            var go = new GameObject("EndingManager");
            _instance = go.AddComponent<EndingManager>();
            _instance._startTime = Time.time;
        }

        private void HandleEscape()
        {
            if (_ended) return;
            _ended = true;

            float elapsed = Time.time - _startTime;
            if (elapsed <= targetSeconds)
            {
                HintUI.Show("Поздравляем! Вы выбрались вовремя.", 15f);
            }
            else
            {
                StartCoroutine(LateEndingSequence());
            }
        }

        private IEnumerator LateEndingSequence()
        {
            HintUI.Show("Вас не спасти - прощайте", 4f);
            yield return new WaitForSeconds(4f);
            HintUI.Show("Субъект пропал без вести...", 60f);
        }
    }
}
