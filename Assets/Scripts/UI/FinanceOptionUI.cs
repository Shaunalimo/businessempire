using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Reusable row for finance/marketing/research options.
    /// </summary>
    public class FinanceOptionUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI detailText;
        [SerializeField] private Button actionButton;

        public void Setup(string title, string cost, string detail, Action onAction)
        {
            if (titleText != null) titleText.text = title;
            if (costText != null) costText.text = cost;
            if (detailText != null) detailText.text = detail;

            actionButton?.onClick.RemoveAllListeners();
            actionButton?.onClick.AddListener(() => onAction?.Invoke());
        }
    }
}
