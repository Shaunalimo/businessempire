using BusinessEmpire.Managers;
using BusinessEmpire.Monetization;
using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 14: Daily Rewards — base claim is free; ad multiplier is optional.
    /// </summary>
    public class DailyRewardsScreen : UIScreenBase
    {
        [SerializeField] private Transform gridContainer;
        [SerializeField] private UIListRowView dayRowPrefab;
        [SerializeField] private TextMeshProUGUI streakText;
        [SerializeField] private Button claimButton;
        [SerializeField] private Button claimWithAdButton;
        [SerializeField] private Button backButton;

        private bool _claimedThisSession;

        private void Awake()
        {
            screenId = UIScreenId.DailyRewards;
            claimButton?.onClick.AddListener(OnClaim);
            claimWithAdButton?.onClick.AddListener(OnClaimWithAd);
            backButton?.onClick.AddListener(GoBack);
        }

        public override void Refresh()
        {
            _claimedThisSession = false;
            RefreshFromLiveData();
            RefreshFromDummyFallback();
            UpdateClaimButtons();
        }

        private void RefreshFromLiveData()
        {
            if (DailyRewardManager.Instance == null || SaveManager.Instance?.CurrentSave == null)
                return;

            if (streakText != null)
                streakText.text = $"Streak: {SaveManager.Instance.CurrentSave.consecutiveLoginDays} days";
        }

        private void RefreshFromDummyFallback()
        {
            if (streakText != null && streakText.text == "")
                streakText.text = "Streak: 2 days · Claim today's reward!";

            ClearGrid();
            foreach (var day in UIDummyDataProvider.GetDailyRewards())
            {
                var row = Instantiate(dayRowPrefab, gridContainer);
                string status = day.isClaimed ? "Claimed" : day.isToday ? "TODAY" : "Locked";
                row.Bind($"Day {day.day}", status,
                    $"{CurrencyFormatter.Format(day.cash)} + {day.gems} gems",
                    null, day.isToday ? UITheme.AccentWarning : UITheme.BackgroundElevated);
            }
        }

        private void UpdateClaimButtons()
        {
            bool canClaim = DailyRewardManager.Instance == null ||
                            (DailyRewardManager.Instance.CanClaimToday && !_claimedThisSession);

            if (claimButton != null) claimButton.interactable = canClaim;
            if (claimWithAdButton != null)
            {
                claimWithAdButton.interactable = _claimedThisSession || canClaim;
                var label = claimWithAdButton.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                    label.text = _claimedThisSession ? "Watch Ad for Bonus x2" : "Claim + Optional Ad x2";
            }
        }

        private void OnClaim()
        {
            MonetizationManager.Instance?.ClaimDailyReward(success =>
            {
                if (!success) return;
                _claimedThisSession = true;
                AudioManager.Instance?.PlayReward();
                UpdateClaimButtons();
            });
        }

        /// <summary>Optional rewarded ad — never required.</summary>
        private void OnClaimWithAd()
        {
            if (!_claimedThisSession)
            {
                MonetizationManager.Instance?.ClaimDailyReward(success =>
                {
                    if (!success) return;
                    _claimedThisSession = true;
                    MonetizationManager.Instance?.OfferDailyRewardMultiplier(_ => Refresh());
                });
                return;
            }

            MonetizationManager.Instance?.OfferDailyRewardMultiplier(_ =>
            {
                AudioManager.Instance?.PlayReward();
                Refresh();
            });
        }

        private void ClearGrid()
        {
            if (gridContainer == null) return;
            foreach (Transform c in gridContainer) Destroy(c.gameObject);
        }
    }
}
