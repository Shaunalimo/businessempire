using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Reusable list row for businesses, loans, achievements, etc.
    /// </summary>
    public class UIListRowView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Image iconPlaceholder;
        [SerializeField] private Button actionButton;

        public void Setup(TextMeshProUGUI title, TextMeshProUGUI subtitle, TextMeshProUGUI value,
            Image icon, Button action)
        {
            titleText = title;
            subtitleText = subtitle;
            valueText = value;
            iconPlaceholder = icon;
            actionButton = action;
        }

        public void Bind(string title, string subtitle, string value, Action onClick, Color? iconColor = null)
        {
            if (titleText != null) titleText.text = title;
            if (subtitleText != null) subtitleText.text = subtitle ?? "";
            if (valueText != null) valueText.text = value ?? "";

            if (iconPlaceholder != null && iconColor.HasValue)
                iconPlaceholder.color = iconColor.Value;

            if (actionButton != null)
            {
                actionButton.onClick.RemoveAllListeners();
                if (onClick != null) actionButton.onClick.AddListener(() => onClick());
            }
        }
    }
}
