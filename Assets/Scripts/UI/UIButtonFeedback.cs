using UnityEngine;
using UnityEngine.EventSystems;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Provides tactile feedback on button presses (Android/iOS).
    /// </summary>
    public class UIButtonFeedback : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private float pressedScale = 0.95f;
        [SerializeField] private float animationSpeed = 12f;

        private Vector3 _originalScale;
        private Vector3 _goalScale;

        private void Awake()
        {
            if (target == null) target = transform as RectTransform;
            _originalScale = target.localScale;
            _goalScale = _originalScale;
        }

        private void Update()
        {
            target.localScale = Vector3.Lerp(target.localScale, _goalScale, Time.unscaledDeltaTime * animationSpeed);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Managers.AudioManager.Instance?.PlayButtonClick();

            if (Managers.SettingsManager.Instance?.Settings.hapticFeedback != true) return;

#if UNITY_ANDROID && !UNITY_EDITOR
            Handheld.Vibrate();
#endif
            _goalScale = _originalScale * pressedScale;
            Invoke(nameof(ResetScale), 0.1f);
        }

        private void ResetScale() => _goalScale = _originalScale;
    }
}
