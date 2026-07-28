#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
using UnityEngine;

namespace BusinessEmpire.Monetization
{
    /// <summary>
    /// Banner ad wrapper (bottom of screen on menus).
    /// </summary>
    internal sealed class BannerAdController
    {
        private BannerView _bannerView;
        private string _adUnitId;
        private bool _isVisible;

        public bool IsShowing => _isVisible && _bannerView != null;

        public void Configure(string adUnitId) => _adUnitId = adUnitId;

        public void LoadAndShow()
        {
            if (string.IsNullOrEmpty(_adUnitId)) return;
            Destroy();

            _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);
            _bannerView.LoadAd(new AdRequest());
            _bannerView.OnBannerAdLoaded += () => _isVisible = true;
            _bannerView.OnBannerAdLoadFailed += (_, error) =>
                Debug.LogWarning($"[Ads] Banner load failed: {error.GetMessage()}");
        }

        public void Hide()
        {
            _bannerView?.Hide();
            _isVisible = false;
        }

        public void Destroy()
        {
            _bannerView?.Destroy();
            _bannerView = null;
            _isVisible = false;
        }
    }
}
#endif

#if !GOOGLE_MOBILE_ADS
using UnityEngine;

namespace BusinessEmpire.Monetization
{
    internal sealed class BannerAdController
    {
        public bool IsShowing { get; private set; }
        public void Configure(string adUnitId) { }
        public void LoadAndShow()
        {
            IsShowing = true;
            Debug.Log("[Ads] Simulated banner shown");
        }
        public void Hide()
        {
            IsShowing = false;
            Debug.Log("[Ads] Simulated banner hidden");
        }
        public void Destroy() => IsShowing = false;
    }
}
#endif
