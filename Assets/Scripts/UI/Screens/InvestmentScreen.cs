using BusinessEmpire.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 12: Investment — timed financial products.
    /// </summary>
    public class InvestmentScreen : UIScreenBase
    {
        [SerializeField] private Transform listContainer;
        [SerializeField] private UIListRowView rowPrefab;
        [SerializeField] private Button backButton;

        private void Awake()
        {
            screenId = UIScreenId.Investment;
            backButton?.onClick.AddListener(GoBack);
        }

        public override void Refresh()
        {
            ClearList();
            foreach (var inv in UIDummyDataProvider.GetInvestments())
            {
                var row = Instantiate(rowPrefab, listContainer);
                row.Bind(inv.name, $"{inv.returnText} · {inv.days} days",
                    CurrencyFormatter.Format(inv.minAmount) + "+", () => { }, UITheme.AccentSuccess);
            }
        }

        private void ClearList()
        {
            if (listContainer == null) return;
            foreach (Transform c in listContainer) Destroy(c.gameObject);
        }
    }
}
