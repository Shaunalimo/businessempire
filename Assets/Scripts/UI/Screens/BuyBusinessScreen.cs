using BusinessEmpire.Data;
using BusinessEmpire.Managers;
using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 7: Buy Business — purchase confirmation.
    /// </summary>
    public class BuyBusinessScreen : UIScreenBase
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI incomeText;
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button cancelButton;

        private void Awake() => screenId = UIScreenId.BuyBusiness;

        private void Start()
        {
            buyButton?.onClick.AddListener(OnBuy);
            cancelButton?.onClick.AddListener(GoBack);
        }

        public override void Refresh()
        {
            var b = UIGameDataBridge.GetBusinessDetail(UIScreenContext.SelectedBusiness);
            if (titleText != null) titleText.text = $"Acquire {b.name}";
            if (costText != null) costText.text = CurrencyFormatter.Format(b.purchaseCost);
            if (incomeText != null) incomeText.text = $"Est. {CurrencyFormatter.Format(b.dailyIncome)}/day";
            if (descText != null) descText.text = b.description;
        }

        private void OnBuy()
        {
            if (BusinessManager.Instance.TryPurchaseBusiness(UIScreenContext.SelectedBusiness))
            {
                AudioManager.Instance?.PlayPurchase();
                NavigateTo(UIScreenId.BusinessDetail);
                return;
            }
            AudioManager.Instance?.PlayError();
        }
    }
}
