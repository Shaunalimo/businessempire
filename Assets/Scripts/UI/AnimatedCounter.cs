using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Animated counter for cash/gem text changes (mobile polish).
    /// </summary>
    public class AnimatedCounter : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI targetText;
        [SerializeField] private float duration = 0.4f;
        [SerializeField] private bool formatAsCurrency = true;

        private double _currentValue;
        private double _targetValue;
        private float _timer;

        public void SetValue(double value, bool animate = true)
        {
            _targetValue = value;
            if (!animate)
            {
                _currentValue = value;
                UpdateDisplay();
                return;
            }
            _timer = 0f;
        }

        private void Update()
        {
            if (Mathf.Approximately((float)_currentValue, (float)_targetValue)) return;

            _timer += Time.deltaTime;
            float t = Mathf.Clamp01(_timer / duration);
            _currentValue = Mathf.Lerp((float)_currentValue, (float)_targetValue, t);
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (targetText == null) return;
            targetText.text = formatAsCurrency
                ? Utils.CurrencyFormatter.Format(_currentValue)
                : ((int)_currentValue).ToString();
        }
    }
}
