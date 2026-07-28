using System;
using System.Collections.Generic;
using BusinessEmpire.Core;
using BusinessEmpire.Data;
using BusinessEmpire.Utils;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Investment portfolio: timed investments with expected returns.
    /// </summary>
    public class InvestmentManager : SingletonMonoBehaviour<InvestmentManager>
    {
        [Serializable]
        public struct InvestmentOption
        {
            public string id;
            public string name;
            public double minAmount;
            public double returnRate;
            public int maturityDays;
        }

        [SerializeField] private List<InvestmentOption> investmentOptions = new List<InvestmentOption>
        {
            new InvestmentOption { id = "savings", name = "Savings Bond", minAmount = 5000, returnRate = 0.05, maturityDays = 7 },
            new InvestmentOption { id = "stocks", name = "Stock Portfolio", minAmount = 25000, returnRate = 0.12, maturityDays = 14 },
            new InvestmentOption { id = "venture", name = "Venture Capital", minAmount = 100000, returnRate = 0.25, maturityDays = 30 }
        };

        public IReadOnlyList<InvestmentOption> Options => investmentOptions;

        public bool TryInvest(string optionId, double amount)
        {
            var option = investmentOptions.Find(o => o.id == optionId);
            if (string.IsNullOrEmpty(option.id) || amount < option.minAmount) return false;
            if (!EconomyManager.Instance.TrySpendCash(amount)) return false;

            var investment = new InvestmentData
            {
                investmentId = Guid.NewGuid().ToString("N"),
                name = option.name,
                amountInvested = amount,
                expectedReturnRate = option.returnRate,
                daysUntilMaturity = option.maturityDays,
                investedTimestamp = TimeHelper.UnixNow
            };

            SaveManager.Instance.CurrentSave.investments.Add(investment);
            return true;
        }

        public void ProcessDailyMaturity()
        {
            var save = SaveManager.Instance.CurrentSave;
            for (int i = save.investments.Count - 1; i >= 0; i--)
            {
                var inv = save.investments[i];
                inv.daysUntilMaturity--;

                if (inv.daysUntilMaturity <= 0)
                {
                    double payout = inv.amountInvested * (1d + inv.expectedReturnRate);
                    EconomyManager.Instance.AddCash(payout);
                    save.investments.RemoveAt(i);
                }
            }
        }
    }
}
