using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Single row in the IAP shop list.
    /// </summary>
    public class ShopProductUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Button buyButton;
        [SerializeField] private Image accentBar;

        public void Setup(string title, string description, string price, Action onBuy)
        {
            if (titleText != null) titleText.text = title;
            if (descriptionText != null) descriptionText.text = description;
            if (priceText != null) priceText.text = price;
            if (accentBar != null) accentBar.color = UITheme.AccentPremium;

            buyButton?.onClick.RemoveAllListeners();
            buyButton?.onClick.AddListener(() => onBuy?.Invoke());
        }
    }
}
