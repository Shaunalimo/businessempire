using BusinessEmpire.Core;
using BusinessEmpire.Data;
using BusinessEmpire.Managers;
using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Detailed business management: purchase, upgrade, hire, profit report.
    /// </summary>
    public class BusinessDetailPanel : MonoBehaviour
    {
        [SerializeField] private UIAnimator animator;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI incomeText;
        [SerializeField] private TextMeshProUGUI salaryText;
        [SerializeField] private TextMeshProUGUI profitText;
        [SerializeField] private TextMeshProUGUI lifetimeProfitText;
        [SerializeField] private Button purchaseButton;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button hireButton;
        [SerializeField] private Button closeButton;

        private BusinessDefinition _currentDefinition;

        private void Awake()
        {
            purchaseButton?.onClick.AddListener(OnPurchase);
            upgradeButton?.onClick.AddListener(OnUpgrade);
            hireButton?.onClick.AddListener(OnHire);
            closeButton?.onClick.AddListener(Hide);
        }

        public void Show(BusinessDefinition definition)
        {
            _currentDefinition = definition;
            animator?.Show();
            Refresh();
        }

        public void Hide() => animator?.Hide();

        private void Refresh()
        {
            if (_currentDefinition == null) return;

            var instance = BusinessManager.Instance.GetInstance(_currentDefinition.type);
            bool owned = instance.isOwned;

            titleText.text = _currentDefinition.displayName;
            descriptionText.text = _currentDefinition.description;
            levelText.text = owned ? $"Level {instance.level}/{GameConstants.MaxBusinessLevel}" : "—";

            double dailyIncome = owned
                ? _currentDefinition.GetDailyIncome(instance.level, instance.employeeCount, 1d, 1d)
                : _currentDefinition.baseDailyIncome;
            double dailySalary = owned
                ? _currentDefinition.GetDailySalaries(instance.employeeCount)
                : 0d;
            double dailyProfit = dailyIncome - dailySalary;

            incomeText.text = CurrencyFormatter.Format(dailyIncome) + "/day";
            salaryText.text = CurrencyFormatter.Format(dailySalary) + "/day";
            profitText.text = CurrencyFormatter.Format(dailyProfit, includeSign: true) + "/day";
            lifetimeProfitText.text = owned
                ? CurrencyFormatter.Format(instance.NetProfit) + " lifetime"
                : "—";

            purchaseButton.gameObject.SetActive(!owned);
            upgradeButton.gameObject.SetActive(owned);
            hireButton.gameObject.SetActive(owned);

            if (!owned)
            {
                purchaseButton.GetComponentInChildren<TextMeshProUGUI>().text =
                    $"Buy {CurrencyFormatter.Format(_currentDefinition.purchaseCost)}";
                purchaseButton.interactable = BusinessManager.Instance.CanPurchase(_currentDefinition.type);
            }
            else
            {
                double upgradeCost = _currentDefinition.GetUpgradeCost(instance.level);
                upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text =
                    $"Upgrade {CurrencyFormatter.Format(upgradeCost)}";
                upgradeButton.interactable = instance.level < GameConstants.MaxBusinessLevel
                    && EconomyManager.Instance.CanAfford(upgradeCost);

                hireButton.GetComponentInChildren<TextMeshProUGUI>().text =
                    $"Hire {CurrencyFormatter.Format(EmployeeManager.Instance.GetHireCost(_currentDefinition.type))}";
                hireButton.interactable = instance.employeeCount < _currentDefinition.maxEmployees;
            }
        }

        private void OnPurchase()
        {
            if (BusinessManager.Instance.TryPurchaseBusiness(_currentDefinition.type))
            {
                AudioManager.Instance?.PlayPurchase();
                Refresh();
            }
        }

        private void OnUpgrade()
        {
            if (BusinessManager.Instance.TryUpgradeBusiness(_currentDefinition.type))
            {
                AudioManager.Instance?.PlayPurchase();
                Refresh();
            }
        }

        private void OnHire()
        {
            if (EmployeeManager.Instance.TryHireEmployee(_currentDefinition.type))
            {
                AudioManager.Instance?.PlayPurchase();
                Refresh();
            }
        }
    }
}
