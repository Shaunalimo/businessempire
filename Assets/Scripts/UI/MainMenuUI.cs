using BusinessEmpire.Core;
using BusinessEmpire.Managers;
using BusinessEmpire.Monetization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Main menu with play, shop, settings, and monetization entry points.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button removeAdsButton;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private SettingsPanel settingsPanel;
        [SerializeField] private ShopPanel shopPanel;
        [SerializeField] private string gameSceneName = "GameScene";

        private void Start()
        {
            GameBootstrap.EnsureManagersExist();
            AudioManager.Instance?.PlayMainMenuMusic();

            if (titleText != null)
                titleText.text = GameConstants.GameTitle;

            playButton?.onClick.AddListener(OnPlayClicked);
            shopButton?.onClick.AddListener(() => shopPanel?.Show());
            settingsButton?.onClick.AddListener(() => settingsPanel?.Show());
            removeAdsButton?.onClick.AddListener(() => MonetizationManager.Instance?.PurchaseRemoveAds());

            if (removeAdsButton != null && SaveManager.Instance.HasSave)
                removeAdsButton.gameObject.SetActive(!SaveManager.Instance.CurrentSave.adsRemoved);
        }

        private void OnPlayClicked()
        {
            AudioManager.Instance?.PlayButtonClick();
            AudioManager.Instance?.PlayGameplayMusic();
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
        }
    }
}
