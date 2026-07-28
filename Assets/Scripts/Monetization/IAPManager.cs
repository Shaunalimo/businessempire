using System;
using System.Collections.Generic;
using BusinessEmpire.Core;
using BusinessEmpire.Data;
using BusinessEmpire.Managers;
using UnityEngine;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
#endif

namespace BusinessEmpire.Monetization
{
    /// <summary>
    /// Unity IAP for Remove Ads, premium currency, and Starter Pack.
    /// Product IDs are placeholders — create matching products in Google Play Console.
    /// </summary>
    public class IAPManager : SingletonMonoBehaviour<IAPManager>
#if UNITY_PURCHASING
        , IDetailedStoreListener
#endif
    {
        [SerializeField] private IapProductCatalog productCatalog;

        public event Action<string> OnPurchaseSucceeded;
        public event Action<string, string> OnPurchaseFailed;

        public bool IsStoreReady { get; private set; }

#if UNITY_PURCHASING
        private IStoreController _storeController;
        private IExtensionProvider _extensionProvider;
        private readonly Dictionary<string, IapProductCatalog.ProductEntry> _catalogMap = new Dictionary<string, IapProductCatalog.ProductEntry>();
#endif

        protected override void OnSingletonAwake()
        {
            if (productCatalog == null)
                productCatalog = Resources.Load<IapProductCatalog>("IapProductCatalog");
            InitializePurchasing();
        }

        private void InitializePurchasing()
        {
#if UNITY_PURCHASING
            if (IsStoreReady) return;
            _catalogMap.Clear();
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            RegisterCatalogProducts(builder);
            UnityPurchasing.Initialize(this, builder);
#else
            IsStoreReady = true;
            Debug.Log("[IAPManager] Simulation mode — enable Unity Purchasing for device tests");
#endif
        }

#if UNITY_PURCHASING
        private void RegisterCatalogProducts(ConfigurationBuilder builder)
        {
            if (productCatalog != null)
            {
                foreach (var entry in productCatalog.products)
                {
                    _catalogMap[entry.productId] = entry;
                    builder.AddProduct(entry.productId,
                        entry.isNonConsumable ? ProductType.NonConsumable : ProductType.Consumable);
                }
                return;
            }

            builder.AddProduct(MonetizationConstants.ProductRemoveAds, ProductType.NonConsumable);
            builder.AddProduct(MonetizationConstants.ProductStarterPack, ProductType.NonConsumable);
            builder.AddProduct(MonetizationConstants.ProductGems100, ProductType.Consumable);
            builder.AddProduct(MonetizationConstants.ProductGems500, ProductType.Consumable);
            builder.AddProduct(MonetizationConstants.ProductGems1200, ProductType.Consumable);
        }
#endif

        public void PurchaseRemoveAds() => PurchaseProduct(MonetizationConstants.ProductRemoveAds);
        public void PurchaseStarterPack() => PurchaseProduct(MonetizationConstants.ProductStarterPack);
        public void PurchaseProduct(string productId) => PurchaseProductInternal(productId);

        public void RestorePurchases()
        {
#if UNITY_PURCHASING
            if (_extensionProvider == null) return;
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
            {
                _extensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((ok, err) =>
                    Debug.Log(ok ? "[IAPManager] Restore OK" : $"[IAPManager] Restore failed: {err}"));
            }
#else
            Debug.Log("[IAPManager] Simulated restore");
#endif
        }

        public string GetLocalizedPrice(string productId)
        {
#if UNITY_PURCHASING
            var product = _storeController?.products.WithID(productId);
            if (product != null && product.availableToPurchase)
                return product.metadata.localizedPriceString;
#endif
            return "Test Buy";
        }

        public bool IsProductOwned(string productId)
        {
            if (productId == MonetizationConstants.ProductRemoveAds)
                return SaveManager.Instance?.CurrentSave?.adsRemoved ?? false;

#if UNITY_PURCHASING
            var product = _storeController?.products.WithID(productId);
            return product != null && product.hasReceipt;
#else
            return false;
#endif
        }

        private void PurchaseProductInternal(string productId)
        {
#if UNITY_PURCHASING
            if (_storeController == null)
            {
                OnPurchaseFailed?.Invoke(productId, "Store not ready");
                return;
            }
            var product = _storeController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
                _storeController.InitiatePurchase(product);
            else
                OnPurchaseFailed?.Invoke(productId, "Product unavailable");
#else
            SimulatePurchase(productId);
#endif
        }

#if UNITY_PURCHASING
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _extensionProvider = extensions;
            IsStoreReady = true;
            if (IsProductOwned(MonetizationConstants.ProductRemoveAds))
                ApplyRemoveAds();
        }

        public void OnInitializeFailed(InitializationFailureReason error) =>
            Debug.LogError($"[IAPManager] Init failed: {error}");

        public void OnInitializeFailed(InitializationFailureReason error, string message) =>
            Debug.LogError($"[IAPManager] Init failed: {error} — {message}");

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            FulfillPurchase(args.purchasedProduct.definition.id);
            OnPurchaseSucceeded?.Invoke(args.purchasedProduct.definition.id);
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason) =>
            OnPurchaseFailed?.Invoke(product.definition.id, reason.ToString());

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription description) =>
            OnPurchaseFailed?.Invoke(product.definition.id, description.message);
#endif

        private void SimulatePurchase(string productId)
        {
            Debug.Log($"[IAPManager] Simulated purchase: {productId}");
            FulfillPurchase(productId);
            OnPurchaseSucceeded?.Invoke(productId);
        }

        private void FulfillPurchase(string productId)
        {
            if (productId == MonetizationConstants.ProductRemoveAds)
            {
                ApplyRemoveAds();
                return;
            }

            if (productId == MonetizationConstants.ProductStarterPack)
            {
                EconomyManager.Instance?.AddCash(MonetizationConstants.StarterPackCash);
                EconomyManager.Instance?.AddPremium(MonetizationConstants.StarterPackGems);
                return;
            }

            int gems = GetGemAmount(productId);
            if (gems > 0)
                EconomyManager.Instance?.AddPremium(gems);
        }

        private void ApplyRemoveAds()
        {
            if (SaveManager.Instance?.CurrentSave == null) return;
            SaveManager.Instance.CurrentSave.adsRemoved = true;
            AdsManager.Instance?.DisableAllAds();
        }

        private int GetGemAmount(string productId)
        {
            if (productCatalog != null)
            {
                var entry = productCatalog.products.Find(p => p.productId == productId);
                if (!string.IsNullOrEmpty(entry.productId))
                    return entry.gemAmount;
            }

            if (productId == MonetizationConstants.ProductGems100) return 100;
            if (productId == MonetizationConstants.ProductGems500) return 500;
            if (productId == MonetizationConstants.ProductGems1200) return 1200;
            return 0;
        }
    }
}
