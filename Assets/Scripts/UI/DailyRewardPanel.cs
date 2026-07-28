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
    /// Daily reward popup — base claim is free; watching an ad is optional for a bonus multiplier.
    /// </summary>
    public class DailyRewardPanel : MonoBehaviour
    {
        [SerializeField] private UIAnimator animator;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private TextMeshProUGUI streakText;
        [SerializeField] private Button claimButton;
        [SerializeField] private Button claimWithAdButton;
        [SerializeField] private Button closeButton;

        private bool _claimedThisSession;

        private void Awake()
        {
            claimButton?.onClick.AddListener(OnClaim);
            claimWithAdButton?.onClick.AddListener(OnClaimWithAd);
            closeButton?.onClick.AddListener(Hide);
        }

        public void Show()
        {
            _claimedThisSession = false;
            animator?.Show();
            Refresh();
        }

        public void Hide() => animator?.Hide();

        private void Refresh()
        {
            var preview = DailyRewardManager.Instance.GetCurrentRewardPreview();
            rewardText.text = $"Day {DailyRewardManager.Instance.CurrentRewardDay}: " +
                              $"{CurrencyFormatter.Format(preview.cashReward)} + {preview.gemReward} gems";
            streakText.text = $"Streak: {SaveManager.Instance.CurrentSave.consecutiveLoginDays} days";

            bool canClaim = DailyRewardManager.Instance.CanClaimToday && !_claimedThisSession;
            claimButton.interactable = canClaim;
            claimWithAdButton.interactable = _claimedThisSession || canClaim;
            if (claimWithAdButton != null)
            {
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
                Refresh();
            });
        }

        /// <summary>Optional rewarded ad — never required. Grants bonus duplicate after base claim.</summary>
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
    }
}
