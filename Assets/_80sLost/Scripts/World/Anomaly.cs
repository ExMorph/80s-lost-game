using UnityEngine;

namespace Lost80s
{
    /// <summary>
    /// Marks this object (and its renderers) as only visible through the camcorder
    /// overlay camera. Drop on anything that should be invisible to the naked eye.
    /// </summary>
    [DisallowMultipleComponent]
    public class Anomaly : MonoBehaviour
    {
        private const string AnomalyLayerName = "Anomaly";

        [Tooltip("Also move child renderers onto the Anomaly layer, not just this object.")]
        [SerializeField] private bool applyToChildren = true;

        private void Awake()
        {
            int layer = LayerMask.NameToLayer(AnomalyLayerName);
            if (layer < 0)
            {
                Debug.LogWarning($"'{AnomalyLayerName}' layer not found. Add it in Project Settings > Tags and Layers.", this);
                return;
            }

            gameObject.layer = layer;
            if (applyToChildren)
            {
                foreach (Transform child in GetComponentsInChildren<Transform>(true))
                {
                    child.gameObject.layer = layer;
                }
            }
        }
    }
}
