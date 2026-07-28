using BusinessEmpire.Core;
using BusinessEmpire.Data;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Audio playback for SFX and music with volume from settings.
    /// </summary>
    public class AudioManager : SingletonMonoBehaviour<AudioManager>
    {
        [SerializeField] private SoundLibrary soundLibrary;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private bool playMusicOnStart = true;

        private static int _sfxPlayCount;

        protected override void OnSingletonAwake()
        {
            if (soundLibrary == null)
                soundLibrary = Resources.Load<SoundLibrary>("SoundLibrary");

            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            ApplyVolumes();
        }

        private void Start()
        {
            if (playMusicOnStart)
                PlayGameplayMusic();
        }

        public void ApplyVolumes()
        {
            if (SaveManager.Instance?.CurrentSave?.settings == null) return;
            var settings = SaveManager.Instance.CurrentSave.settings;
            if (musicSource != null) musicSource.volume = settings.masterVolume * 0.55f;
            if (sfxSource != null) sfxSource.volume = settings.masterVolume * settings.sfxVolume;
        }

        public void PlayMainMenuMusic() => PlayMusic(soundLibrary?.mainMenuMusic);
        public void PlayGameplayMusic() => PlayMusic(soundLibrary?.gameplayMusic);

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null || musicSource == null) return;
            if (musicSource.clip == clip && musicSource.isPlaying) return;
            musicSource.clip = clip;
            musicSource.Play();
        }

        public void StopMusic()
        {
            if (musicSource != null) musicSource.Stop();
        }

        // --- SFX shortcuts ---

        public void PlayButtonClick() => PlaySfx(soundLibrary?.buttonClick);
        public void PlayPanelOpen() => PlaySfx(soundLibrary?.panelOpen, 0.7f);
        public void PlayPanelClose() => PlaySfx(soundLibrary?.panelClose, 0.6f);
        public void PlayError() => PlaySfx(soundLibrary?.error);
        public void PlayPurchase() => PlaySfx(soundLibrary?.purchase);
        public void PlayUpgrade() => PlaySfx(soundLibrary?.upgrade);
        public void PlayCoinCollect() => PlaySfx(soundLibrary?.coinCollect, 0.5f);
        public void PlayCashRegister() => PlaySfx(soundLibrary?.cashRegister);
        public void PlayReward() => PlaySfx(soundLibrary?.reward);
        public void PlayAchievement() => PlaySfx(soundLibrary?.achievement);
        public void PlayLevelUp() => PlaySfx(soundLibrary?.levelUp);
        public void PlayDayAdvance() => PlaySfx(soundLibrary?.dayAdvance, 0.8f);
        public void PlayPositiveEvent() => PlaySfx(soundLibrary?.positiveEvent);
        public void PlayNegativeEvent() => PlaySfx(soundLibrary?.negativeEvent);

        public void PlaySfx(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null || sfxSource == null) return;

            // Limit overlapping identical rapid-fire sounds on mobile
            volumeScale *= 1f - Mathf.Min(_sfxPlayCount * 0.05f, 0.3f);
            sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
            _sfxPlayCount++;
            Invoke(nameof(ResetSfxCount), 0.15f);
        }

        private void ResetSfxCount() => _sfxPlayCount = Mathf.Max(0, _sfxPlayCount - 1);
    }
}
