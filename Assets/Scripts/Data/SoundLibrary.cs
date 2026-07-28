using UnityEngine;

namespace BusinessEmpire.Data
{
    /// <summary>
    /// Central registry of all game sound effects and music.
    /// Create via Assets > Create > Business Empire > Sound Library.
    /// </summary>
    [CreateAssetMenu(fileName = "SoundLibrary", menuName = "Business Empire/Sound Library")]
    public class SoundLibrary : ScriptableObject
    {
        [Header("UI")]
        public AudioClip buttonClick;
        public AudioClip panelOpen;
        public AudioClip panelClose;
        public AudioClip error;

        [Header("Economy")]
        public AudioClip purchase;
        public AudioClip upgrade;
        public AudioClip coinCollect;
        public AudioClip cashRegister;

        [Header("Progression")]
        public AudioClip reward;
        public AudioClip achievement;
        public AudioClip levelUp;
        public AudioClip dayAdvance;

        [Header("Events")]
        public AudioClip positiveEvent;
        public AudioClip negativeEvent;

        [Header("Music")]
        public AudioClip mainMenuMusic;
        public AudioClip gameplayMusic;
    }
}
