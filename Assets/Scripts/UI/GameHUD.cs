using BusinessEmpire.Core;
using BusinessEmpire.Managers;
using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Top HUD showing cash, gems, day counter, and quick navigation.
    /// </summary>
    public class GameHUD : MonoBehaviour
    {
        [Header("Currency")]
        [SerializeField] private TextMeshProUGUI cashText;
        [SerializeField] private TextMeshProUGUI gemsText;
        [SerializeField] private TextMeshProUGUI dayText;

        [Header("Day Progress")]
        [SerializeField] private Slider dayProgressSlider;

        [Header("Navigation")]
        [SerializeField] private Button businessesButton;
        [SerializeField] private Button financeButton;
        [SerializeField] private Button marketingButton;
        [SerializeField] private Button dailyRewardButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button settingsButton;

        [Header("Panels")]
        [SerializeField] private BusinessListPanel businessListPanel;
        [SerializeField] private FinancePanel financePanel;
        [SerializeField] private MarketingPanel marketingPanel;
        [SerializeField] private SettingsPanel settingsPanel;
        [SerializeField] private DailyRewardPanel dailyRewardPanel;
        [SerializeField] private ShopPanel shopPanel;
        [SerializeField] private LeaderboardPanel leaderboardPanel;
        [SerializeField] private ProfitReportPanel profitReportPanel;

        private void OnEnable()
        {
            GameEvents.OnCashChanged += UpdateCash;
            GameEvents.OnPremiumCurrencyChanged += UpdateGems;
            GameEvents.OnDayAdvanced += OnDayAdvanced;
        }

        private void OnDisable()
        {
            GameEvents.OnCashChanged -= UpdateCash;
            GameEvents.OnPremiumCurrencyChanged -= UpdateGems;
            GameEvents.OnDayAdvanced -= OnDayAdvanced;
        }

        private void Start()
        {
            businessesButton?.onClick.AddListener(() => businessListPanel?.Show());
            financeButton?.onClick.AddListener(() => financePanel?.Show());
            marketingButton?.onClick.AddListener(() => marketingPanel?.Show());
            dailyRewardButton?.onClick.AddListener(() => dailyRewardPanel?.Show());
            shopButton?.onClick.AddListener(() => shopPanel?.Show());
            settingsButton?.onClick.AddListener(OnSettingsClicked);

            RefreshAll();

            if (DailyRewardManager.Instance != null && DailyRewardManager.Instance.CanClaimToday)
                dailyRewardPanel?.Show();
        }

        private void Update()
        {
            if (dayProgressSlider != null && GameManager.Instance != null)
                dayProgressSlider.value = GameManager.Instance.DayProgress;
        }

        private void OnSettingsClicked()
        {
            settingsPanel?.Show();
        }

        private void OnDayAdvanced()
        {
            UpdateDay();
            profitReportPanel?.Show();
        }

        private void RefreshAll()
        {
            var save = SaveManager.Instance.CurrentSave;
            UpdateCash(save.cash);
            UpdateGems(save.premiumCurrency);
            UpdateDay();
        }

        private void UpdateCash(double cash)
        {
            if (cashText != null) cashText.text = CurrencyFormatter.Format(cash);
        }

        private void UpdateGems(int gems)
        {
            if (gemsText != null) gemsText.text = gems.ToString();
        }

        private void UpdateDay()
        {
            if (dayText != null)
                dayText.text = $"Day {SaveManager.Instance.CurrentSave.totalDaysPlayed}";
        }
    }
}
