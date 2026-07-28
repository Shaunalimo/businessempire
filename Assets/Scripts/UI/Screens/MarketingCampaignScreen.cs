using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 10: Marketing Campaign — launch promotions.
    /// </summary>
    public class MarketingCampaignScreen : UIScreenBase
    {
        [SerializeField] private Transform listContainer;
        [SerializeField] private UIListRowView rowPrefab;
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private Button backButton;

        private void Awake()
        {
            screenId = UIScreenId.MarketingCampaign;
            backButton?.onClick.AddListener(GoBack);
        }

        public override void Refresh()
        {
            if (headerText != null) headerText.text = "Boost customer growth";
            ClearList();
            foreach (var m in UIDummyDataProvider.GetMarketingOptions())
            {
                var row = Instantiate(rowPrefab, listContainer);
                row.Bind(m.name, m.description, $"{CurrencyFormatter.Format(m.cost)} · {m.bonusText}",
                    () => { }, UITheme.AccentWarning);
            }
        }

        private void ClearList()
        {
            if (listContainer == null) return;
            foreach (Transform c in listContainer) Destroy(c.gameObject);
        }
    }
}
