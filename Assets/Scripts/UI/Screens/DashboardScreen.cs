using BusinessEmpire.Monetization;
using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 4: Player Dashboard — empire overview hub.
    /// </summary>
    public class DashboardScreen : UIScreenBase
    {
        [SerializeField] private UICardView[] statCards;
        [SerializeField] private TextMeshProUGUI welcomeText;
        [SerializeField] private Button dailyRewardButton;
        [SerializeField] private Button achievementsButton;
        [SerializeField] private Button marketingButton;
        [SerializeField] private Button loansButton;
        [SerializeField] private Button investmentsButton;
        [SerializeField] private Button offlinePreviewButton;

        private void Awake() => screenId = UIScreenId.Dashboard;

        private void Start()
        {
            dailyRewardButton?.onClick.AddListener(() => NavigateTo(UIScreenId.DailyRewards));
            achievementsButton?.onClick.AddListener(() => NavigateTo(UIScreenId.Achievements));
            marketingButton?.onClick.AddListener(() => NavigateTo(UIScreenId.MarketingCampaign));
            loansButton?.onClick.AddListener(() => NavigateTo(UIScreenId.BankLoan));
            investmentsButton?.onClick.AddListener(() => NavigateTo(UIScreenId.Investment));
            offlinePreviewButton?.onClick.AddListener(() => ShowPopup(UIScreenId.OfflineEarningsPopup));
        }

        public override void OnScreenShown()
        {
            base.OnScreenShown();
            MonetizationManager.Instance?.EnterGameplay();
        }

        public override void Refresh()
        {
            var player = UIGameDataBridge.GetPlayerHeader();
            var stats = UIGameDataBridge.GetDashboardStats();

            if (welcomeText != null)
                welcomeText.text = $"Welcome back, {player.playerName}";

            if (statCards != null && statCards.Length >= 4)
            {
                statCards[0].Bind("Daily Profit", CurrencyFormatter.Format(stats.dailyProfit, true), "Today", UITheme.AccentSuccess);
                statCards[1].Bind("Businesses", stats.businessesOwned.ToString(), "Owned", UITheme.AccentPrimary);
                statCards[2].Bind("Employees", stats.totalEmployees.ToString(), "Total staff", UITheme.AccentSecondary);
                statCards[3].Bind("Net Worth", CurrencyFormatter.Format(stats.netWorth), stats.topBusiness, UITheme.AccentPremium);
            }
        }
    }
}
