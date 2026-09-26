using UnityEngine;
using UnityEngine.Events;

namespace Lost80s
{
    /// <summary>
    /// The entity is only "real" while framed in the camcorder. Keep it in frame
    /// too long and it fixates on the player. Attach to the entity root; requires
    /// an Anomaly component somewhere in the hierarchy (or set its layer manually).
    /// </summary>
    public class EntityFixation : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Left empty, resolves to CamcorderController.Instance at runtime.")]
        [SerializeField] private Transform viewPoint;

        [Header("Fixation")]
        [Tooltip("Seconds the entity can stay framed before it fixates on the player.")]
        [SerializeField] private float fixationThreshold = 3f;
        [Tooltip("How fast fixation drains once the entity is out of frame.")]
        [SerializeField] private float decayPerSecond = 0.5f;
        [SerializeField] private LayerMask obstructionMask = ~0;

        public float FixationNormalized => Mathf.Clamp01(_fixation / fixationThreshold);
        public bool IsFixated { get; private set; }

        public UnityEvent onFixated;
        public UnityEvent onLostFixation;

        private float _fixation;

        private void Reset()
        {
            viewPoint = transform;
        }

        private void Update()
        {
            CamcorderController camcorder = CamcorderController.Instance;
            if (camcorder == null || camcorder.OverlayCamera == null) return;

            bool framed = camcorder.IsRaised && IsFramed(camcorder.OverlayCamera);
            _fixation += (framed ? 1f : -decayPerSecond) * Time.deltaTime;
            _fixation = Mathf.Clamp(_fixation, 0f, fixationThreshold);

            if (!IsFixated && _fixation >= fixationThreshold)
            {
                IsFixated = true;
                onFixated?.Invoke();
            }
            else if (IsFixated && _fixation <= 0f)
            {
                IsFixated = false;
                onLostFixation?.Invoke();
            }
        }

        private bool IsFramed(Camera overlayCamera)
        {
            Transform point = viewPoint != null ? viewPoint : transform;
            Vector3 viewportPos = overlayCamera.WorldToViewportPoint(point.position);

            bool inFrustum = viewportPos.z > 0f
                              && viewportPos.x > 0f && viewportPos.x < 1f
                              && viewportPos.y > 0f && viewportPos.y < 1f;
            if (!inFrustum) return false;

            Vector3 origin = overlayCamera.transform.position;
            Vector3 target = point.position;
            if (Physics.Linecast(origin, target, out RaycastHit hit, obstructionMask))
            {
                return hit.transform == point || hit.transform.IsChildOf(transform);
            }

            return true;
        }
    }
}
