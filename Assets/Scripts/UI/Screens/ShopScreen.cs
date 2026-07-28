using BusinessEmpire.Data;
using BusinessEmpire.Managers;
using BusinessEmpire.Monetization;
using BusinessEmpire.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 17: Shop — premium currency and IAP products.
    /// </summary>
    public class ShopScreen : UIScreenBase
    {
        [SerializeField] private Transform listContainer;
        [SerializeField] private UIListRowView rowPrefab;
        [SerializeField] private TextMeshProUGUI gemsBalanceText;
        [SerializeField] private Button backButton;

        private IapProductCatalog _catalog;

        private void Awake()
        {
            screenId = UIScreenId.Shop;
            _catalog = Resources.Load<IapProductCatalog>("IapProductCatalog");
        }

        private void Start() => backButton?.onClick.AddListener(GoBack);

        public override void Refresh()
        {
            var header = UIGameDataBridge.GetPlayerHeader();
            if (gemsBalanceText != null) gemsBalanceText.text = $"Your gems: {header.gems}";
            ClearList();

            if (_catalog != null)
            {
                foreach (var product in _catalog.products)
                {
                    if (product.productId == MonetizationConstants.ProductRemoveAds &&
                        SaveManager.Instance?.CurrentSave?.adsRemoved == true)
                        continue;

                    var row = Instantiate(rowPrefab, listContainer);
                    string price = MonetizationManager.Instance?.GetLocalizedPrice(product.productId) ?? "Test Buy";
                    row.Bind(product.displayName, product.description, price,
                        () => OnBuy(product), UITheme.AccentPremium);
                }
                return;
            }

            foreach (var p in UIDummyDataProvider.GetShopProducts())
            {
                var row = Instantiate(rowPrefab, listContainer);
                row.Bind(p.name, p.description, p.price, () => { }, UITheme.AccentPremium);
            }
        }

        private void OnBuy(IapProductCatalog.ProductEntry product)
        {
            AudioManager.Instance?.PlayButtonClick();

            if (product.productId == MonetizationConstants.ProductRemoveAds)
                MonetizationManager.Instance?.PurchaseRemoveAds();
            else if (product.productId == MonetizationConstants.ProductStarterPack)
                MonetizationManager.Instance?.PurchaseStarterPack();
            else
                MonetizationManager.Instance?.PurchaseGemPack(product.productId);
        }

        private void ClearList()
        {
            if (listContainer == null) return;
            foreach (Transform c in listContainer) Destroy(c.gameObject);
        }
    }
}
