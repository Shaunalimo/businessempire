using BusinessEmpire.Core;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Subscribes to GameEvents and triggers appropriate sound effects.
    /// Attach to AudioManager GameObject or auto-created by GameBootstrap.
    /// </summary>
    public class AudioEventBridge : MonoBehaviour
    {
        private double _lastCash;

        private void OnEnable()
        {
            GameEvents.OnCashChanged += OnCashChanged;
            GameEvents.OnBusinessPurchased += OnBusinessPurchased;
            GameEvents.OnBusinessUpgraded += OnBusinessUpgraded;
            GameEvents.OnAchievementUnlocked += OnAchievement;
            GameEvents.OnDailyRewardClaimed += OnDailyReward;
            GameEvents.OnDayAdvanced += OnDayAdvanced;
            GameEvents.OnBusinessEventTriggered += OnBusinessEvent;
            GameEvents.OnOfflineIncomeCollected += OnOfflineIncome;
            GameEvents.OnGameLoaded += OnGameLoaded;
        }

        private void OnDisable()
        {
            GameEvents.OnCashChanged -= OnCashChanged;
            GameEvents.OnBusinessPurchased -= OnBusinessPurchased;
            GameEvents.OnBusinessUpgraded -= OnBusinessUpgraded;
            GameEvents.OnAchievementUnlocked -= OnAchievement;
            GameEvents.OnDailyRewardClaimed -= OnDailyReward;
            GameEvents.OnDayAdvanced -= OnDayAdvanced;
            GameEvents.OnBusinessEventTriggered -= OnBusinessEvent;
            GameEvents.OnOfflineIncomeCollected -= OnOfflineIncome;
            GameEvents.OnGameLoaded -= OnGameLoaded;
        }

        private void OnGameLoaded()
        {
            if (SaveManager.Instance?.CurrentSave != null)
                _lastCash = SaveManager.Instance.CurrentSave.cash;
        }

        private void OnCashChanged(double newCash)
        {
            if (newCash > _lastCash + 100d)
                AudioManager.Instance?.PlayCoinCollect();
            _lastCash = newCash;
        }

        private void OnBusinessPurchased() => AudioManager.Instance?.PlayPurchase();
        private void OnBusinessUpgraded() => AudioManager.Instance?.PlayUpgrade();
        private void OnAchievement(string id) => AudioManager.Instance?.PlayAchievement();
        private void OnDailyReward() => AudioManager.Instance?.PlayReward();
        private void OnDayAdvanced() => AudioManager.Instance?.PlayDayAdvance();

        private void OnBusinessEvent(string id, string message)
        {
            if (id == "strike" || id == "inspection")
                AudioManager.Instance?.PlayNegativeEvent();
            else
                AudioManager.Instance?.PlayPositiveEvent();
        }

        private void OnOfflineIncome(double amount)
        {
            if (amount > 0) AudioManager.Instance?.PlayCashRegister();
        }
    }
}
