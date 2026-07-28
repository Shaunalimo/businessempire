using BusinessEmpire.Core;
using BusinessEmpire.Data;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Manages player cash, premium currency, and financial transactions.
    /// </summary>
    public class EconomyManager : SingletonMonoBehaviour<EconomyManager>
    {
        public double Cash => SaveManager.Instance?.CurrentSave?.cash ?? 0d;
        public int PremiumCurrency => SaveManager.Instance?.CurrentSave?.premiumCurrency ?? 0;

        public bool TrySpendCash(double amount)
        {
            if (amount <= 0) return true;
            var save = SaveManager.Instance.CurrentSave;
            if (save.cash < amount) return false;

            save.cash -= amount;
            GameEvents.RaiseCashChanged(save.cash);
            return true;
        }

        public void AddCash(double amount)
        {
            if (amount == 0) return;
            var save = SaveManager.Instance.CurrentSave;
            save.cash += amount;
            GameEvents.RaiseCashChanged(save.cash);
        }

        public bool TrySpendPremium(int amount)
        {
            if (amount <= 0) return true;
            var save = SaveManager.Instance.CurrentSave;
            if (save.premiumCurrency < amount) return false;

            save.premiumCurrency -= amount;
            GameEvents.RaisePremiumChanged(save.premiumCurrency);
            return true;
        }

        public void AddPremium(int amount)
        {
            if (amount <= 0) return;
            var save = SaveManager.Instance.CurrentSave;
            save.premiumCurrency += amount;
            GameEvents.RaisePremiumChanged(save.premiumCurrency);
        }

        public bool CanAfford(double cashCost) => Cash >= cashCost;
        public bool CanAffordPremium(int gemCost) => PremiumCurrency >= gemCost;
    }
}
