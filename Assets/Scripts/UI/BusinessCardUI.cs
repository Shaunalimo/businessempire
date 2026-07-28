using System;
using BusinessEmpire.Data;
using BusinessEmpire.Managers;
using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Individual business card in the list view

    /// </summary>
    public class BusinessCardUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI profitText;
        [SerializeField] private Button actionButton;

        private BusinessDefinition _definition;
        private Action<BusinessDefinition> _onClick;

        public void Setup(BusinessDefinition definition, Action<BusinessDefinition> onClick)
        {
            _definition = definition;
            _onClick = onClick;

            if (nameText != null) nameText.text = definition.displayName;
            if (iconImage != null && definition.icon != null) iconImage.sprite = definition.icon;

            var instance = BusinessManager.Instance.GetInstance(definition.type);
            if (instance.isOwned)
            {
                if (statusText != null) statusText.text = $"Level {instance.level} • {instance.employeeCount} staff";
                if (profitText != null) profitText.text = CurrencyFormatter.Format(
                    BusinessManager.Instance.GetProjectedDailyProfit(definition.type), includeSign: true) + "/day";
            }
            else
            {
                if (statusText != null) statusText.text = "Not owned";
                if (profitText != null) profitText.text = CurrencyFormatter.Format(definition.purchaseCost);
            }

            actionButton?.onClick.RemoveAllListeners();
            actionButton?.onClick.AddListener(() => _onClick?.Invoke(_definition));
        }
    }
}
