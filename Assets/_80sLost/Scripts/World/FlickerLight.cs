using UnityEngine;

namespace Lost80s
{
    [RequireComponent(typeof(Light))]
    public class FlickerLight : MonoBehaviour
    {
        [SerializeField] private float minIntensity = 0f;
        [SerializeField] private float maxIntensity = 1.5f;
        [SerializeField] private float minInterval = 0.03f;
        [SerializeField] private float maxInterval = 0.4f;
        [Range(0f, 1f)]
        [SerializeField] private float flickerChance = 0.15f;

        private Light _light;
        private float _timer;
        private float _steadyIntensity;

        private void Awake()
        {
            _light = GetComponent<Light>();
            _steadyIntensity = _light.intensity;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;

            _timer = Random.Range(minInterval, maxInterval);
            _light.intensity = Random.value < flickerChance
                ? Random.Range(minIntensity, maxIntensity)
                : _steadyIntensity;
        }
    }
}
