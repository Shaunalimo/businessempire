using BusinessEmpire.Core;
using BusinessEmpire.Managers;
using BusinessEmpire.Monetization;
using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Popup shown when returning from offline. Collect is free; double via ad is optional.
    /// </summary>
    public class OfflineIncomePopup : MonoBehaviour
    {
        [SerializeField] private UIAnimator animator;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private Button collectButton;
        [SerializeField] private Button doubleWithAdButton;

        private void OnEnable()
        {
            GameEvents.OnOfflineIncomeCollected += ShowPopup;
        }

        private void OnDisable()
        {
            GameEvents.OnOfflineIncomeCollected -= ShowPopup;
        }

        private void Awake()
        {
            collectButton?.onClick.AddListener(OnCollect);
            doubleWithAdButton?.onClick.AddListener(OnDoubleWithAd);
            gameObject.SetActive(false);
        }

        private void ShowPopup(double amount)
        {
            if (amount <= 0) return;
            amountText.text = $"You earned {CurrencyFormatter.Format(amount)} while away!";
            animator?.Show();
        }

        private void OnCollect()
        {
            MonetizationManager.Instance?.CollectOfflineEarningsNormal();
            animator?.Hide();
        }

        /// <summary>Optional rewarded ad — doubles offline earnings; never forced.</summary>
        private void OnDoubleWithAd()
        {
            MonetizationManager.Instance?.OfferDoubleOfflineEarnings(_ => animator?.Hide());
        }
    }
}
