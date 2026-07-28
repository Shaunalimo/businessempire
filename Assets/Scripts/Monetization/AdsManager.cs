using System;
using BusinessEmpire.Core;
using BusinessEmpire.Managers;
using UnityEngine;

#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif

namespace BusinessEmpire.Monetization
{
    /// <summary>
    /// Handles Google AdMob banner, interstitial, and rewarded ads.
    /// Uses TEST ad unit IDs from MonetizationConstants until production release.
    /// Falls back to simulation when the Google Mobile Ads SDK is not installed.
    /// </summary>
    public class AdsManager : SingletonMonoBehaviour<AdsManager>
    {
        [Header("Ad Unit IDs — TEST ONLY (see MonetizationConstants)")]
        [SerializeField] private string bannerAndroid = MonetizationConstants.AdMobBannerAndroid;
        [SerializeField] private string interstitialAndroid = MonetizationConstants.AdMobInterstitialAndroid;
        [SerializeField] private string rewardedAndroid = MonetizationConstants.AdMobRewardedAndroid;
        [SerializeField] private string bannerIos = MonetizationConstants.AdMobBannerIos;
        [SerializeField] private string interstitialIos = MonetizationConstants.AdMobInterstitialIos;
        [SerializeField] private string rewardedIos = MonetizationConstants.AdMobRewardedIos;

        [Header("Interstitial Guardrails")]
        [SerializeField] private float minSecondsBetweenInterstitials = MonetizationConstants.MinSecondsBetweenInterstitials;

        private readonly RewardedAdController _rewarded = new RewardedAdController();
        private readonly InterstitialAdController _interstitial = new InterstitialAdController();
        private readonly BannerAdController _banner = new BannerAdController();

        private float _lastInterstitialTime = -999f;
        private int _daysSinceWeeklyInterstitial;
        private bool _initialized;
        private Action _pendingRewardSuccess;
        private Action _pendingRewardFailed;

        public bool AreAdsRemoved => SaveManager.Instance?.CurrentSave?.adsRemoved ?? false;
        public bool IsRewardedReady => AreAdsRemoved || _rewarded.IsReady;
        public bool IsInterstitialReady => !AreAdsRemoved && _interstitial.IsReady;
        public bool IsBannerVisible => _banner.IsShowing;

        public event Action<RewardedAdPlacement, bool> OnRewardedCompleted;
        public event Action<InterstitialMilestone, bool> OnInterstitialCompleted;
        public event Action<string> OnAdError;

        protected override void OnSingletonAwake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            DestroyAllAds();
            base.OnDestroy();
        }

        private void Initialize()
        {
            if (AreAdsRemoved) return;

#if GOOGLE_MOBILE_ADS
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            MobileAds.Initialize(_ =>
            {
                _initialized = true;
                ConfigureControllers();
                Preload();
                Debug.Log("[AdsManager] Google Mobile Ads initialized (TEST IDs)");
            });
#else
            _initialized = true;
            ConfigureControllers();
            Debug.Log("[AdsManager] Simulation mode — import Google Mobile Ads SDK for device testing");
#endif
        }

        private void ConfigureControllers()
        {
            _rewarded.Configure(GetRewardedId());
            _interstitial.Configure(GetInterstitialId());
            _banner.Configure(GetBannerId());
        }

        public void Preload()
        {
            if (AreAdsRemoved || !_initialized) return;
            _rewarded.Load();
            _interstitial.Load();
        }

        // ── Banner (menus only — call ShowMenuBanner / HideMenuBanner) ───────

        public void ShowMenuBanner()
        {
            if (AreAdsRemoved || !_initialized) return;
            _banner.LoadAndShow();
        }

        public void HideMenuBanner()
        {
            _banner.Hide();
        }

        // ── Rewarded (always optional — caller must use explicit UI button) ─

        /// <summary>
        /// Show optional rewarded ad. onSuccess only fires after user completes the ad.
        /// onFailed fires if ad not loaded or user dismisses without reward.
        /// If Remove Ads is owned, onSuccess fires immediately (no ad shown).
        /// </summary>
        public void ShowRewarded(RewardedAdPlacement placement, Action onSuccess, Action onFailed = null)
        {
            if (AreAdsRemoved)
            {
                Debug.Log($"[AdsManager] Ads removed — granting {placement} reward directly");
                onSuccess?.Invoke();
                OnRewardedCompleted?.Invoke(placement, true);
                return;
            }

            _pendingRewardSuccess = () =>
            {
                onSuccess?.Invoke();
                OnRewardedCompleted?.Invoke(placement, true);
            };
            _pendingRewardFailed = () =>
            {
                onFailed?.Invoke();
                OnRewardedCompleted?.Invoke(placement, false);
                OnAdError?.Invoke($"Rewarded ad unavailable: {placement}");
            };

            if (!_rewarded.IsReady)
            {
                Debug.LogWarning($"[AdsManager] Rewarded not ready for {placement}");
                _pendingRewardFailed?.Invoke();
                Preload();
                return;
            }

            _rewarded.Show(
                onRewardGranted: () =>
                {
                    _pendingRewardSuccess?.Invoke();
                    ClearPendingReward();
                },
                onFailed: () =>
                {
                    _pendingRewardFailed?.Invoke();
                    ClearPendingReward();
                    Preload();
                });
        }

        // ── Interstitial (major milestones only + cooldown) ───────────────────

        /// <summary>
        /// Attempt interstitial after a major milestone. Respects cooldown and Remove Ads.
        /// Returns true if an ad was shown (or simulated).
        /// </summary>
        public bool TryShowInterstitial(InterstitialMilestone milestone)
        {
            if (AreAdsRemoved || !_initialized) return false;
            if (Time.unscaledTime - _lastInterstitialTime < minSecondsBetweenInterstitials)
            {
                Debug.Log($"[AdsManager] Interstitial skipped (cooldown): {milestone}");
                return false;
            }

            if (!_interstitial.TryShow())
            {
                OnAdError?.Invoke($"Interstitial not ready: {milestone}");
                Preload();
                OnInterstitialCompleted?.Invoke(milestone, false);
                return false;
            }

            _lastInterstitialTime = Time.unscaledTime;
            OnInterstitialCompleted?.Invoke(milestone, true);
            Debug.Log($"[AdsManager] Interstitial shown for milestone: {milestone}");
            return true;
        }

        /// <summary>Call from game loop — weekly milestone interstitial only.</summary>
        public void OnDayAdvanced()
        {
            if (AreAdsRemoved) return;
            _daysSinceWeeklyInterstitial++;
            if (_daysSinceWeeklyInterstitial >= MonetizationConstants.DaysBetweenWeeklyInterstitial)
            {
                TryShowInterstitial(InterstitialMilestone.WeeklyProgress);
                _daysSinceWeeklyInterstitial = 0;
            }
        }

        /// <summary>Permanently disable all ad formats after Remove Ads purchase.</summary>
        public void DisableAllAds()
        {
            DestroyAllAds();
            Debug.Log("[AdsManager] All ads disabled (Remove Ads active)");
        }

        private void DestroyAllAds()
        {
            _rewarded.Destroy();
            _interstitial.Destroy();
            _banner.Destroy();
        }

        private void ClearPendingReward()
        {
            _pendingRewardSuccess = null;
            _pendingRewardFailed = null;
        }

        private string GetBannerId() =>
#if UNITY_IOS
            bannerIos;
#else
            bannerAndroid;
#endif

        private string GetInterstitialId() =>
#if UNITY_IOS
            interstitialIos;
#else
            interstitialAndroid;
#endif

        private string GetRewardedId() =>
#if UNITY_IOS
            rewardedIos;
#else
            rewardedAndroid;
#endif
    }
}
