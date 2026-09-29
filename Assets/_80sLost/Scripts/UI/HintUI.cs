using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Lost80s
{
    /// <summary>
    /// Minimal on-screen text overlay, built at runtime so no Canvas prefab has to
    /// be hand-authored. Good enough for prototype hints/subtitles/endings; swap
    /// for real UI art later.
    /// </summary>
    public class HintUI : MonoBehaviour
    {
        private static HintUI _instance;

        private Text _text;
        private Coroutine _hideRoutine;

        public static void Show(string message, float seconds)
        {
            EnsureInstance();
            _instance.ShowInternal(message, seconds);
        }

        private static void EnsureInstance()
        {
            if (_instance != null) return;
            var go = new GameObject("HintUI");
            _instance = go.AddComponent<HintUI>();
            _instance.BuildUI();
        }

        private void BuildUI()
        {
            var canvasGO = new GameObject("HintCanvas");
            canvasGO.transform.SetParent(transform);
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasGO.AddComponent<CanvasScaler>();

            var textGO = new GameObject("HintText");
            textGO.transform.SetParent(canvasGO.transform);
            _text = textGO.AddComponent<Text>();
            _text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _text.fontSize = 30;
            _text.alignment = TextAnchor.MiddleCenter;
            _text.color = Color.white;
            _text.horizontalOverflow = HorizontalWrapMode.Wrap;
            _text.verticalOverflow = VerticalWrapMode.Overflow;

            RectTransform rt = _text.rectTransform;
            rt.anchorMin = new Vector2(0.1f, 0.7f);
            rt.anchorMax = new Vector2(0.9f, 0.92f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            _text.text = string.Empty;
        }

        private void ShowInternal(string message, float seconds)
        {
            _text.text = message;
            if (_hideRoutine != null) StopCoroutine(_hideRoutine);
            if (seconds > 0f)
            {
                _hideRoutine = StartCoroutine(HideAfter(seconds));
            }
        }

        private IEnumerator HideAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            _text.text = string.Empty;
        }
    }
}
