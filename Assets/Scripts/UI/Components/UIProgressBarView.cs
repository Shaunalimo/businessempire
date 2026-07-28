using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Reusable labeled progress bar.
    /// </summary>
    public class UIProgressBarView : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private Image fillImage;

        public void Bind(string label, float value, float max, Color? fillColor = null)
        {
            if (labelText != null) labelText.text = label;
            if (slider != null)
            {
                slider.minValue = 0;
                slider.maxValue = max;
                slider.value = value;
            }
            if (fillImage != null && fillColor.HasValue) fillImage.color = fillColor.Value;
        }
    }
}
