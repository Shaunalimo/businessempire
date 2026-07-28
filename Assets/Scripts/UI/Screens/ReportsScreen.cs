using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 13: Reports — financial performance summary.
    /// </summary>
    public class ReportsScreen : UIScreenBase
    {
        [SerializeField] private TextMeshProUGUI summaryText;
        [SerializeField] private Transform listContainer;
        [SerializeField] private UIListRowView rowPrefab;
        [SerializeField] private Button backButton;
        [SerializeField] private Button gameOverPreviewButton;

        private void Awake()
        {
            screenId = UIScreenId.Reports;
            backButton?.onClick.AddListener(GoBack);
            gameOverPreviewButton?.onClick.AddListener(() => NavigateTo(UIScreenId.GameOver));
        }

        public override void Refresh()
        {
            var report = UIDummyDataProvider.GetReport();
            if (summaryText != null)
                summaryText.text =
                    $"<b>Financial Report</b> · Day {report.daysPlayed}\n\n" +
                    $"Revenue: {CurrencyFormatter.Format(report.totalRevenue)}\n" +
                    $"Expenses: {CurrencyFormatter.Format(report.totalExpenses)}\n" +
                    $"<color=#33D4A0>Net Profit: {CurrencyFormatter.Format(report.netProfit, true)}</color>";

            ClearList();
            foreach (var line in report.lines)
            {
                var row = Instantiate(rowPrefab, listContainer);
                row.Bind(line.name, "Daily profit", CurrencyFormatter.Format(line.profit, true), null, UITheme.AccentPrimary);
            }
        }

        private void ClearList()
        {
            if (listContainer == null) return;
            foreach (Transform c in listContainer) Destroy(c.gameObject);
        }
    }
}
