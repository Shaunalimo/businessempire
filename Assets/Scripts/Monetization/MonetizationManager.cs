using System;
using BusinessEmpire.Core;
using BusinessEmpire.Data;
using BusinessEmpire.Managers;
using UnityEngine;

namespace BusinessEmpire.Monetization
{
    /// <summary>
    /// Central monetization coordinator. Game/UI code should call this class —
    /// not AdsManager or IAPManager directly — to keep placement rules consistent.
    /// </summary>
    public class MonetizationManager : SingletonMonoBehaviour<MonetizationManager>
    {
        [Header("Menu Banner")]
        [SerializeField] private bool showBannerOnMainMenu = true;

        public bool AreAdsRemoved => AdsManager.Instance != null && AdsManager.Instance.AreAdsRemoved;
        public bool IsStoreReady => IAPManager.Instance != null && IAPManager.Instance.IsStoreReady;

        public event Action<string> OnStatusMessage;

        protected override void OnSingletonAwake()
        {
            SubscribeGameEvents();
        }

        protected override void OnDestroy()
        {
            UnsubscribeGameEvents();
            base.OnDestroy();
        }

        private void SubscribeGameEvents()
        {
            GameEvents.OnBusinessPurchased += OnBusinessPurchased;
            GameEvents.OnBusinessUpgradedLevel += OnBusinessUpgradedLevel;
            GameEvents.OnAchievementUnlocked += OnAchievementUnlocked;
            GameEvents.OnDayAdvanced += OnDayAdvanced;
        }

        private void UnsubscribeGameEvents()
        {
            GameEvents.OnBusinessPurchased -= OnBusinessPurchased;
            GameEvents.OnBusinessUpgradedLevel -= OnBusinessUpgradedLevel;
            GameEvents.OnAchievementUnlocked -= OnAchievementUnlocked;
            GameEvents.OnDayAdvanced -= OnDayAdvanced;
        }

        // ── Menu lifecycle ────────────────────────────────────────────────────

        public void EnterMainMenu()
        {
            if (showBannerOnMainMenu) AdsManager.Instance?.ShowMenuBanner();
        }

        public void ExitMainMenu()
        {
            AdsManager.Instance?.HideMenuBanner();
        }

        public void EnterGameplay()
        {
            AdsManager.Instance?.HideMenuBanner();
        }

        // ── IAP shortcuts ───────────────────────────────────────────────────

        public void PurchaseRemoveAds() => IAPManager.Instance?.PurchaseRemoveAds();
        public void PurchaseStarterPack() => IAPManager.Instance?.PurchaseStarterPack();
        public void PurchaseGemPack(string productId) => IAPManager.Instance?.PurchaseProduct(productId);
        public void RestorePurchases() => IAPManager.Instance?.RestorePurchases();
        public string GetLocalizedPrice(string productId) => IAPManager.Instance?.GetLocalizedPrice(productId) ?? "—";

        // ── Optional rewarded placements ────────────────────────────────────

        /// <summary>Double offline earnings — optional rewarded ad.</summary>
        public void OfferDoubleOfflineEarnings(Action<bool> onComplete)
        {
            AdsManager.Instance?.ShowRewarded(
                RewardedAdPlacement.DoubleOfflineEarnings,
                onSuccess: () =>
                {
                    OfflineIncomeManager.Instance?.CollectOfflineIncome(useAdBonus: true);
                    ReportStatus("Offline earnings doubled!");
                    onComplete?.Invoke(true);
                },
                onFailed: () => onComplete?.Invoke(false));
        }

        /// <summary>Collect offline earnings without watching an ad.</summary>
        public void CollectOfflineEarningsNormal()
        {
            OfflineIncomeManager.Instance?.CollectOfflineIncome(useAdBonus: false);
        }

        /// <summary>Grant bonus cash — optional rewarded ad.</summary>
        public void OfferExtraCash(Action<bool> onComplete)
        {
            AdsManager.Instance?.ShowRewarded(
                RewardedAdPlacement.ExtraCash,
                onSuccess: () =>
                {
                    EconomyManager.Instance?.AddCash(MonetizationConstants.ExtraCashReward);
                    ReportStatus($"+{MonetizationConstants.ExtraCashReward:N0} cash earned!");
                    onComplete?.Invoke(true);
                },
                onFailed: () => onComplete?.Invoke(false));
        }

        /// <summary>Skip upgrade wait — optional rewarded ad.</summary>
        public void OfferInstantUpgradeSpeedUp(Action<bool> onComplete)
        {
            AdsManager.Instance?.ShowRewarded(
                RewardedAdPlacement.InstantUpgradeSpeedUp,
                onSuccess: () =>
                {
                    // Hook: apply instant upgrade in BusinessManager when wired to live timers.
                    ReportStatus("Upgrade speed-up applied!");
                    onComplete?.Invoke(true);
                },
                onFailed: () => onComplete?.Invoke(false));
        }

        /// <summary>Claim daily reward, then optionally watch ad for multiplier.</summary>
        public void ClaimDailyReward(Action<bool> onComplete)
        {
            if (!DailyRewardManager.Instance.TryClaimDailyReward())
            {
                onComplete?.Invoke(false);
                return;
            }
            ReportStatus("Daily reward claimed!");
            onComplete?.Invoke(true);
        }

        /// <summary>Optional ad multiplier after base daily reward is claimed.</summary>
        public void OfferDailyRewardMultiplier(Action<bool> onComplete)
        {
            AdsManager.Instance?.ShowRewarded(
                RewardedAdPlacement.DailyRewardMultiplier,
                onSuccess: () =>
                {
                    var preview = DailyRewardManager.Instance.GetCurrentRewardPreview();
                    EconomyManager.Instance?.AddCash(preview.cashReward);
                    EconomyManager.Instance?.AddPremium(preview.gemReward);
                    ReportStatus("Daily reward bonus doubled!");
                    onComplete?.Invoke(true);
                },
                onFailed: () => onComplete?.Invoke(false));
        }

        // ── Test hooks (MonetizationTestPanel) ──────────────────────────────

        public void TestRewardedAd(Action<bool> onComplete = null)
        {
            AdsManager.Instance?.ShowRewarded(
                RewardedAdPlacement.GenericTest,
                onSuccess: () => { ReportStatus("Test rewarded completed"); onComplete?.Invoke(true); },
                onFailed: () => { ReportStatus("Test rewarded failed"); onComplete?.Invoke(false); });
        }

        public void TestInterstitialAd()
        {
            bool shown = AdsManager.Instance?.TryShowInterstitial(InterstitialMilestone.GenericTest) ?? false;
            ReportStatus(shown ? "Test interstitial shown" : "Test interstitial skipped/failed");
        }

        public void TestBannerToggle()
        {
            if (AdsManager.Instance == null) return;
            if (AdsManager.Instance.IsBannerVisible)
            {
                AdsManager.Instance.HideMenuBanner();
                ReportStatus("Banner hidden");
            }
            else
            {
                AdsManager.Instance.ShowMenuBanner();
                ReportStatus("Banner shown");
            }
        }

        // ── Milestone interstitials (not every click) ───────────────────────

        private void OnBusinessPurchased()
        {
            AdsManager.Instance?.TryShowInterstitial(InterstitialMilestone.BusinessPurchased);
        }

        private void OnBusinessUpgradedLevel(int newLevel)
        {
            if (newLevel % MonetizationConstants.UpgradeLevelMilestoneInterval == 0)
                AdsManager.Instance?.TryShowInterstitial(InterstitialMilestone.BusinessUpgraded);
        }

        private void OnAchievementUnlocked(string id)
        {
            AdsManager.Instance?.TryShowInterstitial(InterstitialMilestone.AchievementUnlocked);
        }

        private void OnDayAdvanced()
        {
            AdsManager.Instance?.OnDayAdvanced();
        }

        private void ReportStatus(string message)
        {
            Debug.Log($"[Monetization] {message}");
            OnStatusMessage?.Invoke(message);
        }
    }
}
