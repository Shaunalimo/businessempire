using BusinessEmpire.Data;
using BusinessEmpire.Managers;
using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Marketing campaigns and R&D research panel.
    /// </summary>
    public class MarketingPanel : MonoBehaviour
    {
        [SerializeField] private UIAnimator animator;
        [SerializeField] private Transform campaignContainer;
        [SerializeField] private Transform researchContainer;
        [SerializeField] private FinanceOptionUI optionPrefab;
        [SerializeField] private TMP_Dropdown businessDropdown;
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            closeButton?.onClick.AddListener(Hide);
        }

        public void Show()
        {
            animator?.Show();
            PopulateBusinessDropdown();
            Refresh();
        }

        public void Hide() => animator?.Hide();

        private void PopulateBusinessDropdown()
        {
            if (businessDropdown == null) return;
            businessDropdown.ClearOptions();

            var options = new System.Collections.Generic.List<string>();
            foreach (var instance in BusinessManager.Instance.GetOwnedBusinesses())
            {
                var def = BusinessManager.Instance.GetDefinition(instance.type);
                options.Add(def?.displayName ?? instance.type.ToString());
            }

            businessDropdown.AddOptions(options);
        }

        private BusinessType GetSelectedBusiness()
        {
            int index = businessDropdown != null ? businessDropdown.value : 0;
            int i = 0;
            foreach (var instance in BusinessManager.Instance.GetOwnedBusinesses())
            {
                if (i == index) return instance.type;
                i++;
            }
            return BusinessType.GroceryShop;
        }

        private void Refresh()
        {
            ClearContainer(campaignContainer);
            ClearContainer(researchContainer);

            foreach (var campaign in MarketingManager.Instance.Campaigns)
            {
                var ui = Instantiate(optionPrefab, campaignContainer);
                ui.Setup(campaign.name, CurrencyFormatter.Format(campaign.cost),
                    $"+{(campaign.incomeBonus - 1) * 100:F0}% for {campaign.durationDays} days",
                    () => MarketingManager.Instance.TryLaunchCampaign(campaign.id, GetSelectedBusiness()));
            }

            foreach (var node in ResearchManager.Instance.ResearchTree)
            {
                bool completed = ResearchManager.Instance.IsResearchCompleted(node.id);
                var ui = Instantiate(optionPrefab, researchContainer);
                ui.Setup(node.name, completed ? "Completed" : CurrencyFormatter.Format(node.cost),
                    node.description,
                    () => ResearchManager.Instance.TryPurchaseResearch(node.id));
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
