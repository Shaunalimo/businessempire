using BusinessEmpire.Managers;
using BusinessEmpire.Monetization;
using BusinessEmpire.Services;
using UnityEngine;

namespace BusinessEmpire.Core
{
    /// <summary>
    /// Initializes all persistent managers on first scene load.
    /// Attach to a single GameObject in the Bootstrap scene.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Manager Prefabs (optional — auto-created if null)")]
        [SerializeField] private bool initializeOnAwake = true;

        private void Awake()
        {
            if (!initializeOnAwake) return;
            EnsureManagersExist();
        }

        /// <summary>Touch each singleton to force creation in correct order.</summary>
        public static void EnsureManagersExist()
        {
            _ = SaveManager.Instance;
            _ = EconomyManager.Instance;
            _ = BusinessManager.Instance;
            _ = GameManager.Instance;
            _ = EmployeeManager.Instance;
            _ = LoanManager.Instance;
            _ = InvestmentManager.Instance;
            _ = MarketingManager.Instance;
            _ = ResearchManager.Instance;
            _ = EventManager.Instance;
            _ = AchievementManager.Instance;
            _ = DailyRewardManager.Instance;
            _ = OfflineIncomeManager.Instance;
            _ = SettingsManager.Instance;
            _ = AudioManager.Instance;
            _ = LeaderboardManager.Instance;
            _ = CloudSaveManager.Instance;
            _ = PlayerProgressManager.Instance;
            _ = AdsManager.Instance;
            _ = MonetizationManager.Instance;
            _ = IAPManager.Instance;

            EnsureAudioEventBridge();
            Services.PlayGamesAuthService.Initialize();

            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if UNITY_ANDROID || UNITY_IOS
            Input.multiTouchEnabled = true;
#endif
        }

        private static void EnsureAudioEventBridge()
        {
            var audio = AudioManager.Instance;
            if (audio != null && audio.GetComponent<AudioEventBridge>() == null)
                audio.gameObject.AddComponent<AudioEventBridge>();
        }
    }
}
