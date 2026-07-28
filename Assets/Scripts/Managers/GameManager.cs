using BusinessEmpire.Core;
using BusinessEmpire.Data;
using BusinessEmpire.Utils;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Central game loop controller: day progression, initialization, and session flow.
    /// </summary>
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [Header("Timing")]
        [SerializeField] private float dayDurationSeconds = GameConstants.DayDurationSeconds;
        [SerializeField] private bool autoAdvanceDays = true;

        private float _dayTimer;
        private bool _isInitialized;

        public float DayProgress => dayDurationSeconds > 0 ? _dayTimer / dayDurationSeconds : 0f;
        public int CurrentDay => SaveManager.Instance?.CurrentSave?.totalDaysPlayed ?? 0;
        public bool IsInitialized => _isInitialized;

        protected override void OnSingletonAwake()
        {
            GameEvents.OnGameLoaded += HandleGameLoaded;
        }

        protected override void OnDestroy()
        {
            GameEvents.OnGameLoaded -= HandleGameLoaded;
            base.OnDestroy();
        }

        private void Start()
        {
            InitializeGame();
        }

        private void Update()
        {
            if (!_isInitialized || !autoAdvanceDays) return;

            _dayTimer += Time.deltaTime;
            if (_dayTimer >= dayDurationSeconds)
            {
                _dayTimer = 0f;
                AdvanceDay();
            }
        }

        private void HandleGameLoaded()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            if (SaveManager.Instance?.CurrentSave == null) return;

            OfflineIncomeManager.Instance?.ProcessOfflineIncome();
            DailyRewardManager.Instance?.CheckLoginStreak();
            EventManager.Instance?.RollDailyEvents();

            GameEvents.RaiseCashChanged(SaveManager.Instance.CurrentSave.cash);
            GameEvents.RaisePremiumChanged(SaveManager.Instance.CurrentSave.premiumCurrency);

            _isInitialized = true;
        }

        /// <summary>Process one in-game day: income, expenses, loans, investments.</summary>
        public void AdvanceDay()
        {
            var save = SaveManager.Instance.CurrentSave;
            save.totalDaysPlayed++;

            double dailyProfit = BusinessManager.Instance.ProcessDailyCycle();
            LoanManager.Instance.ProcessDailyPayments();
            InvestmentManager.Instance.ProcessDailyMaturity();
            MarketingManager.Instance.ProcessDailyCampaigns();
            EventManager.Instance.ProcessActiveEvents();

            save.totalLifetimeProfit += dailyProfit;
            GameEvents.RaiseDayAdvanced();
            AchievementManager.Instance?.CheckAchievements();
        }

        public void SetAutoAdvance(bool enabled) => autoAdvanceDays = enabled;

        public void SetDayDuration(float seconds) => dayDurationSeconds = Mathf.Max(10f, seconds);
    }
}
