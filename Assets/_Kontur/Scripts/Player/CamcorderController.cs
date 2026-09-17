using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Kontur
{
    /// <summary>
    /// Core mechanic: holding the camcorder up reveals the Anomaly layer through
    /// a dedicated overlay camera, but drains a limited battery. Release (or run
    /// out of battery) and the overlay camera goes dark again.
    /// </summary>
    public class CamcorderController : MonoBehaviour
    {
        [Header("Overlay")]
        [Tooltip("Second camera, culling mask = Anomaly layer only, stacked as Overlay on the main camera.")]
        [SerializeField] private Camera overlayCamera;

        [Header("Battery")]
        [SerializeField] private float maxBattery = 60f;
        [SerializeField] private float drainPerSecond = 1f;
        [SerializeField] private float rechargeDelay = 0f;

        [Header("Input")]
        [SerializeField] private bool holdToRaise = true;

        public bool IsRaised { get; private set; }
        public float BatteryFraction => Mathf.Clamp01(_battery / maxBattery);
        public bool BatteryDepleted => _battery <= 0f;

        public event Action<bool> RaisedChanged;

        private float _battery;

        private void Awake()
        {
            _battery = maxBattery;
            if (overlayCamera != null)
            {
                overlayCamera.enabled = false;
            }
        }

        private void Update()
        {
            bool wantsRaised = ReadRaiseInput();

            if (wantsRaised && BatteryDepleted)
            {
                wantsRaised = false;
            }

            SetRaised(wantsRaised);

            if (IsRaised)
            {
                _battery = Mathf.Max(0f, _battery - drainPerSecond * Time.deltaTime);
                if (_battery <= 0f)
                {
                    SetRaised(false);
                }
            }
        }

        public void AddBattery(float amount)
        {
            _battery = Mathf.Clamp(_battery + amount, 0f, maxBattery);
        }

        private void SetRaised(bool raised)
        {
            if (raised == IsRaised) return;
            IsRaised = raised;
            if (overlayCamera != null)
            {
                overlayCamera.enabled = raised;
            }
            RaisedChanged?.Invoke(raised);
        }

        private bool ReadRaiseInput()
        {
#if ENABLE_INPUT_SYSTEM
            var mouse = Mouse.current;
            if (mouse == null) return IsRaised;

            return holdToRaise ? mouse.rightButton.isPressed : ToggleOnPress(mouse.rightButton.wasPressedThisFrame);
#else
            return holdToRaise ? Input.GetMouseButton(1) : ToggleOnPress(Input.GetMouseButtonDown(1));
#endif
        }

        private bool ToggleOnPress(bool pressedThisFrame)
        {
            return pressedThisFrame ? !IsRaised : IsRaised;
        }
    }
}
