using System;
using UnityEngine;
using UnityEngine.Events;

namespace BusinessEmpire.Core
{
    /// <summary>
    /// Lightweight in-game event bus for decoupled communication between systems.
    /// </summary>
    public static class GameEvents
    {
        public static UnityAction<double> OnCashChanged;
        public static UnityAction<int> OnPremiumCurrencyChanged;
        public static UnityAction OnDayAdvanced;
        public static UnityAction OnGameSaved;
        public static UnityAction OnGameLoaded;
        public static UnityAction<string> OnAchievementUnlocked;
        public static UnityAction<string, string> OnBusinessEventTriggered;
        public static UnityAction OnBusinessPurchased;
        public static UnityAction OnBusinessUpgraded;
        public static UnityAction<int> OnBusinessUpgradedLevel;
        public static UnityAction OnDailyRewardClaimed;
        public static UnityAction<double> OnOfflineIncomeCollected;

        public static void RaiseCashChanged(double newCash) => OnCashChanged?.Invoke(newCash);
        public static void RaisePremiumChanged(int gems) => OnPremiumCurrencyChanged?.Invoke(gems);
        public static void RaiseDayAdvanced() => OnDayAdvanced?.Invoke();
        public static void RaiseGameSaved() => OnGameSaved?.Invoke();
        public static void RaiseGameLoaded() => OnGameLoaded?.Invoke();
        public static void RaiseAchievement(string id) => OnAchievementUnlocked?.Invoke(id);
        public static void RaiseBusinessEvent(string id, string message) => OnBusinessEventTriggered?.Invoke(id, message);
        public static void RaiseBusinessPurchased() => OnBusinessPurchased?.Invoke();
        public static void RaiseBusinessUpgraded() => OnBusinessUpgraded?.Invoke();
        public static void RaiseBusinessUpgraded(int newLevel) => OnBusinessUpgradedLevel?.Invoke(newLevel);
        public static void RaiseDailyRewardClaimed() => OnDailyRewardClaimed?.Invoke();
        public static void RaiseOfflineIncome(double amount) => OnOfflineIncomeCollected?.Invoke(amount);

        /// <summary>Clear all listeners on scene reload (optional cleanup).</summary>
        public static void ClearAll()
        {
            OnCashChanged = null;
            OnPremiumCurrencyChanged = null;
            OnDayAdvanced = null;
            OnGameSaved = null;
            OnGameLoaded = null;
            OnAchievementUnlocked = null;
            OnBusinessEventTriggered = null;
            OnBusinessPurchased = null;
            OnBusinessUpgraded = null;
            OnBusinessUpgradedLevel = null;
            OnDailyRewardClaimed = null;
            OnOfflineIncomeCollected = null;
        }
    }
}
