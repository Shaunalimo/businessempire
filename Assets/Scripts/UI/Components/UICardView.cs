using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Reusable stat card for dashboard and reports.
    /// </summary>
    public class UICardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private Image accentBar;
        [SerializeField] private Image iconPlaceholder;

        public void Setup(TextMeshProUGUI title, TextMeshProUGUI value, TextMeshProUGUI subtitle,
            Image accent, Image icon)
        {
            titleText = title;
            valueText = value;
            subtitleText = subtitle;
            accentBar = accent;
            iconPlaceholder = icon;
        }

        public void Bind(string title, string value, string subtitle = null, Color? accent = null)
        {
            if (titleText != null) titleText.text = title;
            if (valueText != null) valueText.text = value;
            if (subtitleText != null) subtitleText.text = subtitle ?? "";
            if (accentBar != null && accent.HasValue) accentBar.color = accent.Value;
        }

        public void SetIconColor(Color c)
        {
            if (iconPlaceholder != null) iconPlaceholder.color = c;
        }
    }
}
