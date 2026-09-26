using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Lost80s
{
    /// <summary>
    /// Screen "blink" + a temporary film grain/vignette spike that eases back
    /// down, played whenever the player crosses into a new zone. Auto-creates
    /// itself and finds the scene's Volume the first time it's needed.
    /// </summary>
    public class ZoneEffects : MonoBehaviour
    {
        private static ZoneEffects _instance;

        private Image _flashImage;
        private FilmGrain _filmGrain;
        private Vignette _vignette;
        private ChromaticAberration _chromaticAberration;
        private float _baseGrainIntensity;
        private float _baseVignetteIntensity;
        private float _baseChromaticIntensity;
        private Coroutine _routine;

        public static void Pulse(float grainBoost = 0.6f, float duration = 1f)
        {
            EnsureInstance();
            _instance.PulseInternal(grainBoost, duration);
        }

        private static void EnsureInstance()
        {
            if (_instance != null) return;
            var go = new GameObject("ZoneEffects");
            _instance = go.AddComponent<ZoneEffects>();
            _instance.BuildFlashUI();
            _instance.CacheVolumeComponents();
        }

        private void CacheVolumeComponents()
        {
            var volume = FindObjectOfType<Volume>();
            if (volume == null || volume.profile == null) return;

            if (volume.profile.TryGet(out FilmGrain grain))
            {
                _filmGrain = grain;
                _baseGrainIntensity = grain.intensity.value;
            }
            if (volume.profile.TryGet(out Vignette vignette))
            {
                _vignette = vignette;
                _baseVignetteIntensity = vignette.intensity.value;
            }
            if (volume.profile.TryGet(out ChromaticAberration chromaticAberration))
            {
                _chromaticAberration = chromaticAberration;
                _baseChromaticIntensity = chromaticAberration.intensity.value;
            }
        }

        private void BuildFlashUI()
        {
            var canvasGO = new GameObject("ZoneFlashCanvas");
            canvasGO.transform.SetParent(transform);
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 90;
            canvasGO.AddComponent<CanvasScaler>();

            var imageGO = new GameObject("Flash");
            imageGO.transform.SetParent(canvasGO.transform);
            _flashImage = imageGO.AddComponent<Image>();
            _flashImage.color = new Color(0f, 0f, 0f, 0f);
            _flashImage.raycastTarget = false;

            RectTransform rt = _flashImage.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private void PulseInternal(float grainBoost, float duration)
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(PulseRoutine(grainBoost, duration));
        }

        private IEnumerator PulseRoutine(float grainBoost, float duration)
        {
            const float blinkIn = 0.04f;
            const float blinkHold = 0.08f;
            const float blinkOut = 0.22f;

            for (float t = 0f; t < blinkIn; t += Time.deltaTime)
            {
                SetFlashAlpha(t / blinkIn);
                yield return null;
            }
            SetFlashAlpha(1f);
            yield return new WaitForSeconds(blinkHold);
            for (float t = 0f; t < blinkOut; t += Time.deltaTime)
            {
                SetFlashAlpha(1f - t / blinkOut);
                yield return null;
            }
            SetFlashAlpha(0f);

            // Fixed, clearly-visible peaks rather than a small fraction of the
            // (already fairly grainy) baseline - this is meant to read as a
            // distinct "something just happened" hit, not a subtle nudge.
            float grainPeak = Mathf.Clamp01(_baseGrainIntensity + grainBoost);
            float vignettePeak = Mathf.Clamp01(_baseVignetteIntensity + 0.4f * grainBoost);
            float chromaticPeak = Mathf.Clamp01(_baseChromaticIntensity + 0.7f * grainBoost);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float k = 1f - Mathf.Clamp01(elapsed / duration);
                if (_filmGrain != null) _filmGrain.intensity.value = Mathf.Lerp(_baseGrainIntensity, grainPeak, k);
                if (_vignette != null) _vignette.intensity.value = Mathf.Lerp(_baseVignetteIntensity, vignettePeak, k);
                if (_chromaticAberration != null) _chromaticAberration.intensity.value = Mathf.Lerp(_baseChromaticIntensity, chromaticPeak, k);
                yield return null;
            }

            if (_filmGrain != null) _filmGrain.intensity.value = _baseGrainIntensity;
            if (_vignette != null) _vignette.intensity.value = _baseVignetteIntensity;
            if (_chromaticAberration != null) _chromaticAberration.intensity.value = _baseChromaticIntensity;
        }

        private void SetFlashAlpha(float a)
        {
            if (_flashImage == null) return;
            Color c = _flashImage.color;
            c.a = Mathf.Clamp01(a);
            _flashImage.color = c;
        }
    }
}
