#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
using UnityEngine;

namespace BusinessEmpire.Monetization
{
    /// <summary>
    /// Google Mobile Ads interstitial wrapper.
    /// </summary>
    internal sealed class InterstitialAdController
    {
        private InterstitialAd _interstitialAd;
        private string _adUnitId;
        private bool _isLoading;

        public bool IsReady => _interstitialAd != null && _interstitialAd.CanShowAd();

        public void Configure(string adUnitId) => _adUnitId = adUnitId;

        public void Load()
        {
            if (_isLoading || IsReady || string.IsNullOrEmpty(_adUnitId)) return;
            _isLoading = true;

            InterstitialAd.Load(_adUnitId, new AdRequest(), (ad, error) =>
            {
                _isLoading = false;
                if (error != null)
                {
                    Debug.LogWarning($"[AdMob] Interstitial load failed: {error.GetMessage()}");
                    return;
                }

                _interstitialAd = ad;
                ad.OnAdFullScreenContentClosed += () =>
                {
                    _interstitialAd?.Destroy();
                    _interstitialAd = null;
                    Load();
                };
                ad.OnAdFullScreenContentFailed += _ =>
                {
                    _interstitialAd?.Destroy();
                    _interstitialAd = null;
                    Load();
                };
            });
        }

        public bool TryShow()
        {
            if (!IsReady)
            {
                Load();
                return false;
            }

            _interstitialAd.Show();
            return true;
        }

        public void Destroy()
        {
            _interstitialAd?.Destroy();
            _interstitialAd = null;
        }
    }
}
#endif

#if !GOOGLE_MOBILE_ADS
using UnityEngine;

namespace BusinessEmpire.Monetization
{
    internal sealed class InterstitialAdController
    {
        public bool IsReady => true;
        public void Configure(string adUnitId) { }
        public void Load() { }
        public bool TryShow()
        {
            Debug.Log("[AdMob] Simulated interstitial ad");
            return true;
        }
        public void Destroy() { }
    }
}
#endif
