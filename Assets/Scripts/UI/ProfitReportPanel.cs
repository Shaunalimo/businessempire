using BusinessEmpire.Core;
using BusinessEmpire.Managers;
using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Profit report summary showing per-business and total daily performance.
    /// </summary>
    public class ProfitReportPanel : MonoBehaviour
    {
        [SerializeField] private UIAnimator animator;
        [SerializeField] private TextMeshProUGUI summaryText;
        [SerializeField] private UnityEngine.UI.Button closeButton;

        private void Awake()
        {
            closeButton?.onClick.AddListener(Hide);
            GameEvents.OnDayAdvanced += Refresh;
        }

        private void OnDestroy()
        {
            GameEvents.OnDayAdvanced -= Refresh;
        }

        public void Show()
        {
            animator?.Show();
            Refresh();
        }

        public void Hide() => animator?.Hide();

        private void Refresh()
        {
            if (summaryText == null) return;

            var sb = new System.Text.StringBuilder();
            double total = 0d;

            sb.AppendLine("<b>Daily Profit Report</b>\n");

            foreach (var instance in BusinessManager.Instance.GetOwnedBusinesses())
            {
                var def = BusinessManager.Instance.GetDefinition(instance.type);
                string name = def?.displayName ?? instance.type.ToString();
                sb.AppendLine($"{name}: {CurrencyFormatter.Format(instance.lastDayProfit, includeSign: true)}");
                total += instance.lastDayProfit;
            }

            sb.AppendLine($"\n<b>Total: {CurrencyFormatter.Format(total, includeSign: true)}</b>");
            summaryText.text = sb.ToString();
        }
    }
}
