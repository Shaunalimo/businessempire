using System.Collections.Generic;
using BusinessEmpire.Monetization;
using UnityEngine;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Debug/test panel for monetization — rewarded, interstitial, banner, and IAP.
    /// Attach to a Canvas panel or enable via Settings in prototype builds.
    /// </summary>
    public class MonetizationTestPanel : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI statusText;
        [SerializeField] private UnityEngine.UI.Button rewardedButton;
        [SerializeField] private UnityEngine.UI.Button interstitialButton;
        [SerializeField] private UnityEngine.UI.Button bannerToggleButton;
        [SerializeField] private UnityEngine.UI.Button removeAdsButton;
        [SerializeField] private UnityEngine.UI.Button gemsButton;
        [SerializeField] private UnityEngine.UI.Button starterPackButton;
        [SerializeField] private UnityEngine.UI.Button extraCashRewardButton;
        [SerializeField] private UnityEngine.UI.Button doubleOfflineButton;
        [SerializeField] private UnityEngine.UI.Button dailyMultiplierButton;
        [SerializeField] private UnityEngine.UI.Button closeButton;

        private void Awake()
        {
            rewardedButton?.onClick.AddListener(() =>
                MonetizationManager.Instance?.TestRewardedAd(ok => SetStatus(ok ? "Rewarded ad completed" : "Rewarded ad failed")));
            interstitialButton?.onClick.AddListener(() => MonetizationManager.Instance?.TestInterstitialAd());
            bannerToggleButton?.onClick.AddListener(() => MonetizationManager.Instance?.TestBannerToggle());
            removeAdsButton?.onClick.AddListener(() => MonetizationManager.Instance?.PurchaseRemoveAds());
            gemsButton?.onClick.AddListener(() =>
                MonetizationManager.Instance?.PurchaseGemPack(MonetizationConstants.ProductGems100));
            starterPackButton?.onClick.AddListener(() => MonetizationManager.Instance?.PurchaseStarterPack());
            extraCashRewardButton?.onClick.AddListener(() =>
                MonetizationManager.Instance?.OfferExtraCash(ok => SetStatus(ok ? "Extra cash granted" : "Ad failed")));
            doubleOfflineButton?.onClick.AddListener(() =>
                MonetizationManager.Instance?.OfferDoubleOfflineEarnings(ok => SetStatus(ok ? "Offline doubled" : "Ad failed")));
            dailyMultiplierButton?.onClick.AddListener(() =>
                MonetizationManager.Instance?.OfferDailyRewardMultiplier(ok => SetStatus(ok ? "Daily bonus x2" : "Ad failed")));
            closeButton?.onClick.AddListener(() => gameObject.SetActive(false));
        }

        private void OnEnable()
        {
            if (MonetizationManager.Instance != null)
                MonetizationManager.Instance.OnStatusMessage += SetStatus;
            if (IAPManager.Instance != null)
            {
                IAPManager.Instance.OnPurchaseSucceeded += id => SetStatus($"IAP success: {id}");
                IAPManager.Instance.OnPurchaseFailed += (id, err) => SetStatus($"IAP failed: {id} — {err}");
            }
            SetStatus("Monetization test panel ready (TEST IDs only)");
        }

        private void OnDisable()
        {
            if (MonetizationManager.Instance != null)
                MonetizationManager.Instance.OnStatusMessage -= SetStatus;
        }

        private void SetStatus(string msg)
        {
            if (statusText != null) statusText.text = msg;
            Debug.Log($"[MonetizationTest] {msg}");
        }
    }
}
