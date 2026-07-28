using UnityEngine;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Adjusts RectTransform padding for Android/iOS notch and safe areas.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class UISafeAreaFitter : MonoBehaviour
    {
        [SerializeField] private bool applyTop = true;
        [SerializeField] private bool applyBottom = true;
        private RectTransform _rt;
        private Rect _lastSafeArea;

        private void Awake() => _rt = GetComponent<RectTransform>();

        private void Update()
        {
            if (Screen.safeArea == _lastSafeArea) return;
            Apply();
        }

        private void Apply()
        {
            _lastSafeArea = Screen.safeArea;
            var anchorMin = _lastSafeArea.position;
            var anchorMax = _lastSafeArea.position + _lastSafeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            if (!applyTop) anchorMax.y = 1f;
            if (!applyBottom) anchorMin.y = 0f;

            _rt.anchorMin = anchorMin;
            _rt.anchorMax = anchorMax;
        }
    }
}
