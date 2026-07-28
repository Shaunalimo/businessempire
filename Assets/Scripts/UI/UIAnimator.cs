using System.Collections;
using UnityEngine;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Reusable fade/slide animations for mobile UI panels.
    /// </summary>
    public class UIAnimator : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform panelTransform;
        [SerializeField] private float animationDuration = 0.25f;
        [SerializeField] private float slideOffset = 80f;

        private Coroutine _currentRoutine;

        private void Awake()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (panelTransform == null) panelTransform = transform as RectTransform;
        }

        public void Show()
        {
            BusinessEmpire.Managers.AudioManager.Instance?.PlayPanelOpen();
            gameObject.SetActive(true);
            RunAnimate(ShowRoutine());
        }

        public void Hide(System.Action onComplete = null)
        {
            BusinessEmpire.Managers.AudioManager.Instance?.PlayPanelClose();
            RunAnimate(HideRoutine(onComplete));
        }

        private void RunAnimate(IEnumerator routine)
        {
            if (_currentRoutine != null) StopCoroutine(_currentRoutine);
            _currentRoutine = StartCoroutine(routine);
        }

        private IEnumerator ShowRoutine()
        {
            if (canvasGroup != null) canvasGroup.alpha = 0f;
            Vector2 startPos = panelTransform.anchoredPosition + Vector2.down * slideOffset;
            Vector2 endPos = panelTransform.anchoredPosition;
            panelTransform.anchoredPosition = startPos;

            float t = 0f;
            while (t < animationDuration)
            {
                t += Time.unscaledDeltaTime;
                float progress = Mathf.SmoothStep(0f, 1f, t / animationDuration);
                if (canvasGroup != null) canvasGroup.alpha = progress;
                panelTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, progress);
                yield return null;
            }

            if (canvasGroup != null) canvasGroup.alpha = 1f;
            panelTransform.anchoredPosition = endPos;
        }

        private IEnumerator HideRoutine(System.Action onComplete)
        {
            Vector2 startPos = panelTransform.anchoredPosition;
            Vector2 endPos = startPos + Vector2.down * slideOffset;
            float t = 0f;

            while (t < animationDuration)
            {
                t += Time.unscaledDeltaTime;
                float progress = Mathf.SmoothStep(0f, 1f, t / animationDuration);
                if (canvasGroup != null) canvasGroup.alpha = 1f - progress;
                panelTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, progress);
                yield return null;
            }

            gameObject.SetActive(false);
            onComplete?.Invoke();
        }
    }
}
