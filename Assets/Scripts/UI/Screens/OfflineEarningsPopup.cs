using BusinessEmpire.Managers;
using BusinessEmpire.Monetization;
using BusinessEmpire.UI;
using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 20: Offline Earnings popup — collect is free; x2 via ad is optional.
    /// </summary>
    public class OfflineEarningsPopup : UIPopupBase
    {
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private Button collectButton;
        [SerializeField] private Button doubleButton;

        private void Awake()
        {
            screenId = UIScreenId.OfflineEarningsPopup;
            hideTopBar = true;
            hideBottomNav = true;
            collectButton?.onClick.AddListener(OnCollect);
            doubleButton?.onClick.AddListener(OnDoubleWithAd);
        }

        public override void Refresh()
        {
            double amount = GetDisplayAmount();
            if (amountText != null) amountText.text = CurrencyFormatter.Format(amount);
            if (subtitleText != null)
                subtitleText.text = amount > 0 ? "Earned while you were away" : "No offline earnings pending";
        }

        private static double GetDisplayAmount()
        {
            var offline = OfflineIncomeManager.Instance;
            if (offline != null && offline.PendingOfflineIncome > 0)
                return offline.PendingOfflineIncome;
            return UIDummyDataProvider.GetOfflineEarnings();
        }

        private void OnCollect()
        {
            MonetizationManager.Instance?.CollectOfflineEarningsNormal();
            Close();
        }

        /// <summary>Optional rewarded ad — doubles offline earnings; never forced.</summary>
        private void OnDoubleWithAd()
        {
            MonetizationManager.Instance?.OfferDoubleOfflineEarnings(_ => Close());
        }
    }
}
