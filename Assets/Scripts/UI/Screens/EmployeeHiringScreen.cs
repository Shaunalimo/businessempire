using System.Collections.Generic;
using BusinessEmpire.Data;
using BusinessEmpire.Managers;
using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 9: Employee Hiring — manage staff across businesses.
    /// </summary>
    public class EmployeeHiringScreen : UIScreenBase
    {
        [SerializeField] private Transform listContainer;
        [SerializeField] private UIListRowView rowPrefab;
        [SerializeField] private TextMeshProUGUI summaryText;
        [SerializeField] private Button backButton;

        private void Awake() => screenId = UIScreenId.EmployeeHiring;

        private void Start() => backButton?.onClick.AddListener(GoBack);

        public override void Refresh()
        {
            ClearList();
            int total = 0;
            foreach (var slot in UIGameDataBridge.GetEmployeeSlots())
            {
                total += slot.current;
                var row = Instantiate(rowPrefab, listContainer);
                var type = slot.businessType;
                row.Bind(slot.businessName,
                    $"{slot.current}/{slot.max} · Salary {CurrencyFormatter.Format(slot.salaryPerDay)}/day",
                    $"Hire {CurrencyFormatter.Format(slot.hireCost)}",
                    () =>
                    {
                        if (EmployeeManager.Instance.TryHireEmployee(type))
                        {
                            AudioManager.Instance?.PlayPurchase();
                            Refresh();
                            UINavigationManager.Instance?.RefreshTopBar();
                        }
                        else AudioManager.Instance?.PlayError();
                    },
                    UITheme.AccentSecondary);
            }
            if (summaryText != null) summaryText.text = $"Total employees: {total}";
        }

        private void ClearList()
        {
            if (listContainer == null) return;
            foreach (Transform c in listContainer) Destroy(c.gameObject);
        }
    }
}
