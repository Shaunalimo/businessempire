using BusinessEmpire.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 11: Bank Loan — borrow capital.
    /// </summary>
    public class BankLoanScreen : UIScreenBase
    {
        [SerializeField] private Transform listContainer;
        [SerializeField] private UIListRowView rowPrefab;
        [SerializeField] private Button backButton;

        private void Awake()
        {
            screenId = UIScreenId.BankLoan;
            backButton?.onClick.AddListener(GoBack);
        }

        public override void Refresh()
        {
            ClearList();
            foreach (var loan in UIDummyDataProvider.GetLoans())
            {
                var row = Instantiate(rowPrefab, listContainer);
                row.Bind(loan.name, loan.terms, CurrencyFormatter.Format(loan.amount), () => { }, UITheme.AccentSecondary);
            }
        }

        private void ClearList()
        {
            if (listContainer == null) return;
            foreach (Transform c in listContainer) Destroy(c.gameObject);
        }
    }
}
