using BusinessEmpire.Core;
using BusinessEmpire.Managers;
using BusinessEmpire.Monetization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Settings panel: audio, haptics, reset, cloud save status.
    /// </summary>
    public class SettingsPanel : MonoBehaviour
    {
        [SerializeField] private UIAnimator animator;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle hapticToggle;
        [SerializeField] private Toggle notificationsToggle;
        [SerializeField] private TextMeshProUGUI cloudSaveStatusText;
        [SerializeField] private Button removeAdsButton;
        [SerializeField] private Button restorePurchasesButton;
        [SerializeField] private Button leaderboardButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private LeaderboardPanel leaderboardPanel;

        private void Awake()
        {
            closeButton?.onClick.AddListener(Hide);
            resetButton?.onClick.AddListener(OnReset);
            removeAdsButton?.onClick.AddListener(() => MonetizationManager.Instance?.PurchaseRemoveAds());
            restorePurchasesButton?.onClick.AddListener(() => MonetizationManager.Instance?.RestorePurchases());
            leaderboardButton?.onClick.AddListener(() =>
            {
                Hide();
                leaderboardPanel?.Show();
            });
        }

        public void Show()
        {
            animator?.Show();
            LoadSettings();
        }

        public void Hide() => animator?.Hide();

        private void LoadSettings()
        {
            var settings = SettingsManager.Instance.Settings;
            masterVolumeSlider.value = settings.masterVolume;
            sfxVolumeSlider.value = settings.sfxVolume;
            hapticToggle.isOn = settings.hapticFeedback;
            notificationsToggle.isOn = settings.notificationsEnabled;

            masterVolumeSlider.onValueChanged.RemoveAllListeners();
            sfxVolumeSlider.onValueChanged.RemoveAllListeners();
            hapticToggle.onValueChanged.RemoveAllListeners();
            notificationsToggle.onValueChanged.RemoveAllListeners();

            masterVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.SetMasterVolume);
            sfxVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.SetSfxVolume);
            hapticToggle.onValueChanged.AddListener(SettingsManager.Instance.SetHapticFeedback);
            notificationsToggle.onValueChanged.AddListener(SettingsManager.Instance.SetNotifications);

            cloudSaveStatusText.text = CloudSaveManager.Instance.LastSyncStatus;
            removeAdsButton.gameObject.SetActive(!SaveManager.Instance.CurrentSave.adsRemoved);
        }

        private void OnReset()
        {
            SettingsManager.Instance.ResetProgress();
            Hide();
        }
    }
}
