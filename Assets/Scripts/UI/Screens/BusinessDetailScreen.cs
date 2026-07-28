using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 6: Business Detail — manage an owned business.
    /// </summary>
    public class BusinessDetailScreen : UIScreenBase
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private TextMeshProUGUI statsText;
        [SerializeField] private Image iconPlaceholder;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button hireButton;
        [SerializeField] private Button marketingButton;
        [SerializeField] private Button backButton;

        private void Awake() => screenId = UIScreenId.BusinessDetail;

        private void Start()
        {
            upgradeButton?.onClick.AddListener(() => NavigateTo(UIScreenId.UpgradeBusiness));
            hireButton?.onClick.AddListener(() => NavigateTo(UIScreenId.EmployeeHiring));
            marketingButton?.onClick.AddListener(() => NavigateTo(UIScreenId.MarketingCampaign));
            backButton?.onClick.AddListener(GoBack);
        }

        public override void Refresh()
        {
            var b = UIGameDataBridge.GetBusinessDetail(UIScreenContext.SelectedBusiness);
            if (titleText != null) titleText.text = b.name;
            if (descText != null) descText.text = b.description;
            if (iconPlaceholder != null) iconPlaceholder.color = UITheme.AccentSecondary;
            if (statsText != null)
                statsText.text =
                    $"Level {b.level}\n" +
                    $"Employees: {b.employees}\n" +
                    $"Income: {CurrencyFormatter.Format(b.dailyIncome)}/day\n" +
                    $"Profit: {CurrencyFormatter.Format(b.dailyProfit, true)}/day";
        }
    }
}
