#if GOOGLE_MOBILE_ADS
using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace BusinessEmpire.Monetization
{
    /// <summary>
    /// Google Mobile Ads rewarded ad wrapper (requires GOOGLE_MOBILE_ADS define).
    /// </summary>
    internal sealed class RewardedAdController
    {
        private RewardedAd _rewardedAd;
        private string _adUnitId;
        private bool _isLoading;

        public bool IsReady => _rewardedAd != null && _rewardedAd.CanShowAd();

        public void Configure(string adUnitId) => _adUnitId = adUnitId;

        public void Load()
        {
            if (_isLoading || IsReady || string.IsNullOrEmpty(_adUnitId)) return;
            _isLoading = true;

            RewardedAd.Load(_adUnitId, new AdRequest(), (ad, error) =>
            {
                _isLoading = false;
                if (error != null)
                {
                    Debug.LogWarning($"[AdMob] Rewarded load failed: {error.GetMessage()}");
                    return;
                }

                _rewardedAd = ad;
                RegisterEvents(ad);
            });
        }

        public void Show(Action onRewardGranted, Action onFailed)
        {
            if (!IsReady)
            {
                onFailed?.Invoke();
                Load();
                return;
            }

            _rewardedAd.Show(reward =>
            {
                Debug.Log($"[AdMob] Reward granted: {reward.Type} x{reward.Amount}");
                onRewardGranted?.Invoke();
                _rewardedAd = null;
                Load();
            });
        }

        private void RegisterEvents(RewardedAd ad)
        {
            ad.OnAdFullScreenContentClosed += () =>
            {
                _rewardedAd = null;
                Load();
            };
            ad.OnAdFullScreenContentFailed += _ =>
            {
                _rewardedAd = null;
                Load();
            };
        }

        public void Destroy()
        {
            _rewardedAd?.Destroy();
            _rewardedAd = null;
        }
    }
}
#endif

#if !GOOGLE_MOBILE_ADS
using System;
using UnityEngine;

namespace BusinessEmpire.Monetization
{
    /// <summary>
    /// Simulated rewarded ad when Google Mobile Ads SDK is not installed.
    /// </summary>
    internal sealed class RewardedAdController
    {
        public bool IsReady => true;
        public void Configure(string adUnitId) { }
        public void Load() { }
        public void Show(Action onRewardGranted, Action onFailed)
        {
            Debug.Log("[AdMob] Simulated rewarded ad");
            onRewardGranted?.Invoke();
        }
        public void Destroy() { }
    }
}
#endif
