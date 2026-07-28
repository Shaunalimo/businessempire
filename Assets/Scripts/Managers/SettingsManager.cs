using BusinessEmpire.Core;
using BusinessEmpire.Data;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Player preferences: audio, haptics, notifications, language.
    /// </summary>
    public class SettingsManager : SingletonMonoBehaviour<SettingsManager>
    {
        public GameSettingsData Settings => SaveManager.Instance.CurrentSave.settings;

        public void SetMasterVolume(float volume)
        {
            Settings.masterVolume = Mathf.Clamp01(volume);
            AudioManager.Instance?.ApplyVolumes();
        }

        public void SetSfxVolume(float volume)
        {
            Settings.sfxVolume = Mathf.Clamp01(volume);
            AudioManager.Instance?.ApplyVolumes();
        }

        public void SetNotifications(bool enabled) => Settings.notificationsEnabled = enabled;
        public void SetHapticFeedback(bool enabled) => Settings.hapticFeedback = enabled;
        public void SetLanguage(string languageCode) => Settings.languageCode = languageCode;

        public void ResetProgress()
        {
            SaveManager.Instance.DeleteSave();
        }
    }
}
