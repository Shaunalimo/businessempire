using BusinessEmpire.Core;
using BusinessEmpire.Managers;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Player level and XP progression based on business performance.
    /// </summary>
    public class PlayerProgressManager : SingletonMonoBehaviour<PlayerProgressManager>
    {
        [SerializeField] private double xpPerProfitDollar = 0.001d;
        [SerializeField] private double baseXpToLevel = 1000d;
        [SerializeField] private double xpLevelMultiplier = 1.5d;

        protected override void OnSingletonAwake()
        {
            GameEvents.OnDayAdvanced += OnDayAdvanced;
        }

        protected override void OnDestroy()
        {
            GameEvents.OnDayAdvanced -= OnDayAdvanced;
            base.OnDestroy();
        }

        private void OnDayAdvanced()
        {
            var save = SaveManager.Instance.CurrentSave;
            double dailyProfit = 0d;
            foreach (var b in BusinessManager.Instance.GetOwnedBusinesses())
                dailyProfit += b.lastDayProfit;

            if (dailyProfit > 0)
                AddXp(dailyProfit * xpPerProfitDollar);
        }

        public void AddXp(double amount)
        {
            if (amount <= 0) return;
            var save = SaveManager.Instance.CurrentSave;
            save.playerXp += amount;

            while (save.playerXp >= GetXpRequiredForLevel(save.playerLevel))
            {
                save.playerXp -= GetXpRequiredForLevel(save.playerLevel);
                save.playerLevel++;
            }
        }

        public double GetXpRequiredForLevel(int level) =>
            baseXpToLevel * System.Math.Pow(xpLevelMultiplier, level - 1);

        public float LevelProgress =>
            (float)(SaveManager.Instance.CurrentSave.playerXp / GetXpRequiredForLevel(SaveManager.Instance.CurrentSave.playerLevel));
    }
}
