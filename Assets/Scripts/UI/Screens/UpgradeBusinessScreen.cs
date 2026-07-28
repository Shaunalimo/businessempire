using BusinessEmpire.Core;
using BusinessEmpire.Data;
using BusinessEmpire.Managers;
using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 8: Upgrade Business — level up confirmation.
    /// </summary>
    public class UpgradeBusinessScreen : UIScreenBase
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI currentLevelText;
        [SerializeField] private TextMeshProUGUI upgradeCostText;
        [SerializeField] private TextMeshProUGUI bonusText;
        [SerializeField] private UIProgressBarView levelProgress;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button backButton;

        private void Awake() => screenId = UIScreenId.UpgradeBusiness;

        private void Start()
        {
            upgradeButton?.onClick.AddListener(OnUpgrade);
            backButton?.onClick.AddListener(GoBack);
        }

        public override void Refresh()
        {
            var b = UIGameDataBridge.GetBusinessDetail(UIScreenContext.SelectedBusiness);
            var def = BusinessManager.Instance.GetDefinition(UIScreenContext.SelectedBusiness);
            if (titleText != null) titleText.text = $"Upgrade {b.name}";
            if (currentLevelText != null) currentLevelText.text = $"Level {b.level} → {b.level + 1}";
            if (upgradeCostText != null && def != null)
                upgradeCostText.text = CurrencyFormatter.Format(def.GetUpgradeCost(b.level));
            if (bonusText != null) bonusText.text = "+15% daily income";
            levelProgress?.Bind("Level progress", b.level, GameConstants.MaxBusinessLevel, UITheme.AccentPrimary);
        }

        private void OnUpgrade()
        {
            if (BusinessManager.Instance.TryUpgradeBusiness(UIScreenContext.SelectedBusiness))
            {
                AudioManager.Instance?.PlayUpgrade();
                GoBack();
                return;
            }
            AudioManager.Instance?.PlayError();
        }
    }
}
