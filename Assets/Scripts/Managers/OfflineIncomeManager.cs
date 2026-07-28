using BusinessEmpire.Core;
using BusinessEmpire.Data;
using BusinessEmpire.Utils;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Calculates and grants income earned while the app was closed.
    /// </summary>
    public class OfflineIncomeManager : SingletonMonoBehaviour<OfflineIncomeManager>
    {
        [SerializeField] private float offlineEfficiency = 0.5f;

        public double PendingOfflineIncome { get; private set; }

        public void ProcessOfflineIncome()
        {
            var save = SaveManager.Instance.CurrentSave;
            double hoursAway = TimeHelper.HoursSince(save.lastOnlineTimestamp);
            if (hoursAway < 0.5d) return;

            hoursAway = System.Math.Min(hoursAway, GameConstants.OfflineIncomeCapHours);

            double dailyProfit = CalculateDailyProfitEstimate();
            double daysAway = hoursAway / 24d;
            PendingOfflineIncome = dailyProfit * daysAway * offlineEfficiency;

            if (PendingOfflineIncome > 0)
                GameEvents.RaiseOfflineIncome(PendingOfflineIncome);
        }

        public void CollectOfflineIncome(bool useAdBonus = false)
        {
            if (PendingOfflineIncome <= 0) return;

            double amount = PendingOfflineIncome;
            if (useAdBonus) amount *= 2d;

            EconomyManager.Instance.AddCash(amount);
            PendingOfflineIncome = 0;
        }

        private double CalculateDailyProfitEstimate()
        {
            double total = 0d;
            foreach (var instance in BusinessManager.Instance.GetOwnedBusinesses())
            {
                total += BusinessManager.Instance.GetProjectedDailyProfit(instance.type);
            }
            return System.Math.Max(0d, total);
        }
    }
}
