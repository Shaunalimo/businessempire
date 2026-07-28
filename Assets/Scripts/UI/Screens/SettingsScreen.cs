using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 16: Settings — audio, haptics, monetization test tools.
    /// </summary>
    public class SettingsScreen : UIScreenBase
    {
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle hapticToggle;
        [SerializeField] private Toggle notificationsToggle;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button gameOverPreviewButton;
        [SerializeField] private Button monetizationTestButton;
        [SerializeField] private TextMeshProUGUI versionText;

        private void Awake()
        {
            screenId = UIScreenId.Settings;
            hideBottomNav = true;
        }

        private void Start()
        {
            backButton?.onClick.AddListener(GoBack);
            resetButton?.onClick.AddListener(OnReset);
        }

        private void OnReset()
        {
            Managers.SaveManager.Instance?.DeleteSave();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        public override void Refresh()
        {
            if (masterVolumeSlider != null) masterVolumeSlider.value = 0.8f;
            if (sfxVolumeSlider != null) sfxVolumeSlider.value = 1f;
            if (hapticToggle != null) hapticToggle.isOn = true;
            if (notificationsToggle != null) notificationsToggle.isOn = true;
            if (versionText != null) versionText.text = "Business Empire v1.0.0 · Prototype UI";
        }

        private static void OpenMonetizationTestPanel()
        {
            var panel = GameObject.Find("MonetizationTestPanel");
            if (panel != null)
            {
                panel.SetActive(true);
                return;
            }

            Debug.LogWarning("[Settings] MonetizationTestPanel not found. Run Business Empire → Setup Monetization SDKs.");
        }
    }
}
