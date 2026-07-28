using BusinessEmpire.Core;
using BusinessEmpire.Data;
using BusinessEmpire.Managers;
using BusinessEmpire.Monetization;
using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// In-game shop for gem packs and Remove Ads purchase.
    /// </summary>
    public class ShopPanel : MonoBehaviour
    {
        [SerializeField] private UIAnimator animator;
        [SerializeField] private Transform productContainer;
        [SerializeField] private ShopProductUI productPrefab;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private Button closeButton;
        [SerializeField] private IapProductCatalog catalog;

        private void Awake()
        {
            closeButton?.onClick.AddListener(Hide);
        }

        private void OnEnable()
        {
            if (IAPManager.Instance != null)
                IAPManager.Instance.OnPurchaseSucceeded += OnPurchaseSucceeded;
        }

        private void OnDisable()
        {
            if (IAPManager.Instance != null)
                IAPManager.Instance.OnPurchaseSucceeded -= OnPurchaseSucceeded;
        }

        public void Show()
        {
            animator?.Show();
            Refresh();
        }

        public void Hide() => animator?.Hide();

        private void Refresh()
        {
            if (productContainer == null || productPrefab == null) return;

            foreach (Transform child in productContainer)
                Destroy(child.gameObject);

            if (catalog == null)
            {
                SetStatus("Shop catalog not assigned.");
                return;
            }

            foreach (var product in catalog.products)
            {
                if (product.productId == GameConstants.RemoveAdsProductId &&
                    SaveManager.Instance.CurrentSave.adsRemoved)
                    continue;

                var ui = Instantiate(productPrefab, productContainer);
                string price = IAPManager.Instance?.GetLocalizedPrice(product.productId) ?? "—";
                ui.Setup(product.displayName, product.description, price, () => OnBuy(product));
            }

            SetStatus(IAPManager.Instance != null && IAPManager.Instance.IsStoreReady
                ? "Secure payments via Google Play"
                : "Store initializing…");
        }

        private void OnBuy(IapProductCatalog.ProductEntry product)
        {
            AudioManager.Instance?.PlayButtonClick();

            if (product.productId == GameConstants.RemoveAdsProductId)
                MonetizationManager.Instance?.PurchaseRemoveAds();
            else if (product.productId == MonetizationConstants.ProductStarterPack)
                MonetizationManager.Instance?.PurchaseStarterPack();
            else
                MonetizationManager.Instance?.PurchaseGemPack(product.productId);
        }

        private void OnPurchaseSucceeded(string productId)
        {
            SetStatus("Purchase successful! Thank you.");
            Refresh();
        }

        private void SetStatus(string message)
        {
            if (statusText != null) statusText.text = message;
        }
    }
}
