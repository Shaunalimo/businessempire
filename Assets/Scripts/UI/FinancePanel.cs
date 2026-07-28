using BusinessEmpire.Managers;
using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Finance hub: loans and investments.
    /// </summary>
    public class FinancePanel : MonoBehaviour
    {
        [SerializeField] private UIAnimator animator;
        [SerializeField] private Transform loanContainer;
        [SerializeField] private Transform investmentContainer;
        [SerializeField] private FinanceOptionUI optionPrefab;
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            closeButton?.onClick.AddListener(Hide);
        }

        public void Show()
        {
            animator?.Show();
            Refresh();
        }

        public void Hide() => animator?.Hide();

        private void Refresh()
        {
            ClearContainer(loanContainer);
            ClearContainer(investmentContainer);

            foreach (var loan in LoanManager.Instance.AvailableLoans)
            {
                var ui = Instantiate(optionPrefab, loanContainer);
                ui.Setup(loan.name, CurrencyFormatter.Format(loan.amount),
                    $"{loan.termDays} days • {(loan.dailyInterestRate * 100):F1}% daily",
                    () => LoanManager.Instance.TryTakeLoan(loan.id));
            }

            foreach (var inv in InvestmentManager.Instance.Options)
            {
                var ui = Instantiate(optionPrefab, investmentContainer);
                ui.Setup(inv.name, CurrencyFormatter.Format(inv.minAmount) + "+",
                    $"+{(inv.returnRate * 100):F0}% in {inv.maturityDays} days",
                    () => InvestmentManager.Instance.TryInvest(inv.id, inv.minAmount));
            }
        }

        private static void ClearContainer(Transform container)
        {
            if (container == null) return;
            foreach (Transform child in container)
                Destroy(child.gameObject);
        }
    }
}
