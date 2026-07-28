using System.Collections.Generic;
using BusinessEmpire.Data;
using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 5: Business List — all businesses owned and available.
    /// </summary>
    public class BusinessListScreen : UIScreenBase
    {
        [SerializeField] private Transform listContainer;
        [SerializeField] private UIListRowView rowPrefab;
        [SerializeField] private Button backButton;

        private void Awake() => screenId = UIScreenId.BusinessList;

        private void Start() => backButton?.onClick.AddListener(GoBack);

        public override void Refresh()
        {
            ClearList();
            foreach (var b in UIGameDataBridge.GetBusinesses())
            {
                var row = Instantiate(rowPrefab, listContainer);
                string status = b.isOwned ? $"Lv.{b.level} · {b.employees} staff" : "Not owned";
                string value = b.isOwned
                    ? CurrencyFormatter.Format(b.dailyProfit, true) + "/day"
                    : CurrencyFormatter.Format(b.purchaseCost);
                var type = b.type;
                row.Bind(b.name, status, value, () => OpenBusiness(type), UITheme.AccentPrimary);
            }
        }

        private void OpenBusiness(BusinessType type)
        {
            UIScreenContext.SelectedBusiness = type;
            var card = UIGameDataBridge.GetBusinessDetail(type);
            NavigateTo(card.isOwned ? UIScreenId.BusinessDetail : UIScreenId.BuyBusiness);
        }

        private void ClearList()
        {
            if (listContainer == null) return;
            foreach (Transform c in listContainer) Destroy(c.gameObject);
        }
    }
}
