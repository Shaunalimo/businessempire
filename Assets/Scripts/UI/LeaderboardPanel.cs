using BusinessEmpire.Managers;
using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Leaderboard panel with Google Play Games sign-in and global scores.
    /// </summary>
    public class LeaderboardPanel : MonoBehaviour
    {
        [SerializeField] private UIAnimator animator;
        [SerializeField] private Transform listContainer;
        [SerializeField] private LeaderboardEntryUI entryPrefab;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button submitScoreButton;
        [SerializeField] private Button signInButton;
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button nativeUiButton;

        private void Awake()
        {
            closeButton?.onClick.AddListener(Hide);
            submitScoreButton?.onClick.AddListener(OnSubmitScore);
            signInButton?.onClick.AddListener(OnSignIn);
            refreshButton?.onClick.AddListener(OnRefresh);
            nativeUiButton?.onClick.AddListener(OnShowNative);
        }

        private void OnEnable()
        {
            if (LeaderboardManager.Instance != null)
                LeaderboardManager.Instance.OnLeaderboardUpdated += Refresh;
        }

        private void OnDisable()
        {
            if (LeaderboardManager.Instance != null)
                LeaderboardManager.Instance.OnLeaderboardUpdated -= Refresh;
        }

        public void Show()
        {
            animator?.Show();
            Refresh();
            LeaderboardManager.Instance?.RefreshOnlineScores();
        }

        public void Hide() => animator?.Hide();

        private void Refresh()
        {
            if (listContainer == null || entryPrefab == null) return;

            foreach (Transform child in listContainer)
                Destroy(child.gameObject);

            foreach (var entry in LeaderboardManager.Instance.GetDisplayLeaderboard())
            {
                var ui = Instantiate(entryPrefab, listContainer);
                string name = entry.isOnline ? entry.playerName : entry.playerName;
                ui.Setup(entry.rank, name, entry.score, entry.isOnline);
            }

            if (statusText != null)
            {
                string loading = LeaderboardManager.Instance.IsLoadingOnline ? " ⏳" : "";
                statusText.text = LeaderboardManager.Instance.StatusMessage + loading;
            }

            if (signInButton != null)
                signInButton.gameObject.SetActive(!LeaderboardManager.Instance.IsOnlineAvailable);
        }

        private void OnSubmitScore()
        {
            AudioManager.Instance?.PlayButtonClick();
            var save = SaveManager.Instance.CurrentSave;
            LeaderboardManager.Instance.SubmitScore(save.totalLifetimeProfit);
        }

        private void OnSignIn()
        {
            AudioManager.Instance?.PlayButtonClick();
            LeaderboardManager.Instance.SignIn(_ => Refresh());
        }

        private void OnRefresh()
        {
            AudioManager.Instance?.PlayButtonClick();
            LeaderboardManager.Instance.RefreshOnlineScores();
        }

        private void OnShowNative()
        {
            AudioManager.Instance?.PlayButtonClick();
            LeaderboardManager.Instance.ShowNativeLeaderboard();
        }
    }
}
