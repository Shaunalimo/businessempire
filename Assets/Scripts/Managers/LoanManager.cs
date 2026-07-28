using System;
using System.Collections.Generic;
using BusinessEmpire.Core;
using BusinessEmpire.Data;
using BusinessEmpire.Utils;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Bank loan system: take loans, daily interest, and repayment.
    /// </summary>
    public class LoanManager : SingletonMonoBehaviour<LoanManager>
    {
        [Serializable]
        public struct LoanOffer
        {
            public string id;
            public string name;
            public double amount;
            public double dailyInterestRate;
            public int termDays;
        }

        [SerializeField] private List<LoanOffer> availableLoans = new List<LoanOffer>
        {
            new LoanOffer { id = "small", name = "Starter Loan", amount = 10000, dailyInterestRate = 0.002, termDays = 30 },
            new LoanOffer { id = "medium", name = "Growth Loan", amount = 50000, dailyInterestRate = 0.0015, termDays = 60 },
            new LoanOffer { id = "large", name = "Enterprise Loan", amount = 250000, dailyInterestRate = 0.001, termDays = 90 }
        };

        public IReadOnlyList<LoanOffer> AvailableLoans => availableLoans;

        public bool TryTakeLoan(string loanId)
        {
            var offer = availableLoans.Find(l => l.id == loanId);
            if (string.IsNullOrEmpty(offer.id)) return false;

            var save = SaveManager.Instance.CurrentSave;
            if (save.activeLoans.Count >= 3) return false;

            var loan = new LoanData
            {
                loanId = Guid.NewGuid().ToString("N"),
                principal = offer.amount,
                remainingBalance = offer.amount,
                dailyInterestRate = offer.dailyInterestRate,
                daysRemaining = offer.termDays,
                takenTimestamp = TimeHelper.UnixNow
            };

            save.activeLoans.Add(loan);
            EconomyManager.Instance.AddCash(offer.amount);
            return true;
        }

        public bool TryRepayLoan(string loanId, double amount)
        {
            var save = SaveManager.Instance.CurrentSave;
            var loan = save.activeLoans.Find(l => l.loanId == loanId);
            if (loan == null) return false;

            double payAmount = Math.Min(amount, loan.remainingBalance);
            if (!EconomyManager.Instance.TrySpendCash(payAmount)) return false;

            loan.remainingBalance -= payAmount;
            if (loan.remainingBalance <= 0.01d)
                save.activeLoans.Remove(loan);

            return true;
        }

        public void ProcessDailyPayments()
        {
            var save = SaveManager.Instance.CurrentSave;
            for (int i = save.activeLoans.Count - 1; i >= 0; i--)
            {
                var loan = save.activeLoans[i];
                double interest = loan.remainingBalance * loan.dailyInterestRate;
                loan.remainingBalance += interest;
                loan.daysRemaining--;

                if (loan.daysRemaining <= 0)
                {
                    EconomyManager.Instance.TrySpendCash(loan.remainingBalance);
                    save.activeLoans.RemoveAt(i);
                }
            }
        }
    }
}
