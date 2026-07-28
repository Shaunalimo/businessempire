using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Persistent top bar: Cash, Gems, Company Level, Reputation.
    /// </summary>
    public class UITopBar : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI cashText;
        [SerializeField] private TextMeshProUGUI gemsText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI reputationText;
        [SerializeField] private Slider reputationSlider;
        [SerializeField] private Button settingsButton;

        private void Awake()
        {
            settingsButton?.onClick.AddListener(() =>
                UINavigationManager.Instance?.ShowScreen(UIScreenId.Settings));
        }

        public void Setup(TextMeshProUGUI cash, TextMeshProUGUI gems, TextMeshProUGUI level,
            TextMeshProUGUI rep, Slider slider, Button settings)
        {
            cashText = cash;
            gemsText = gems;
            levelText = level;
            reputationText = rep;
            reputationSlider = slider;
            settingsButton = settings;
            settingsButton?.onClick.AddListener(() =>
                UINavigationManager.Instance?.ShowScreen(UIScreenId.Settings));
        }

        public void Refresh()
        {
            var data = UIGameDataBridge.GetPlayerHeader();

            if (cashText != null) cashText.text = CurrencyFormatter.Format(data.cash);
            if (gemsText != null) gemsText.text = data.gems.ToString();
            if (levelText != null) levelText.text = $"Lv.{data.companyLevel}";
            if (reputationText != null) reputationText.text = $"{data.reputation}%";
            if (reputationSlider != null)
            {
                reputationSlider.minValue = 0;
                reputationSlider.maxValue = 100;
                reputationSlider.value = data.reputation;
            }
        }
    }
}
