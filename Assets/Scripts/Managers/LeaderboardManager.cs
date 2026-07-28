using System;
using System.Collections.Generic;
using BusinessEmpire.Core;
using BusinessEmpire.Services;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Leaderboard manager: local cache + Google Play Games online scores.
    /// </summary>
    public class LeaderboardManager : SingletonMonoBehaviour<LeaderboardManager>
    {
        [SerializeField] private string primaryLeaderboardId = PlayGamesConstants.LeaderboardTotalProfit;
        [SerializeField] private int maxLocalEntries = 20;
        [SerializeField] private bool autoSubmitOnDayAdvance = true;
        [SerializeField] private bool autoSignInOnStart = true;

        private readonly List<LeaderboardEntry> _localEntries = new List<LeaderboardEntry>();
        private readonly List<PlayGamesLeaderboardService.OnlineEntry> _onlineEntries = new List<PlayGamesLeaderboardService.OnlineEntry>();

        public bool IsOnlineAvailable => PlayGamesLeaderboardService.IsAvailable;
        public bool IsLoadingOnline { get; private set; }
        public string StatusMessage { get; private set; } = "Leaderboard ready";

        public event Action OnLeaderboardUpdated;

        public struct LeaderboardEntry
        {
            public string playerName;
            public double score;
            public int rank;
            public bool isOnline;
        }

        protected override void OnSingletonAwake()
        {
            PlayGamesAuthService.Initialize();
            GameEvents.OnDayAdvanced += OnDayAdvanced;
            PlayGamesAuthService.OnAuthStateChanged += OnAuthChanged;
        }

        protected override void OnDestroy()
        {
            GameEvents.OnDayAdvanced -= OnDayAdvanced;
            PlayGamesAuthService.OnAuthStateChanged -= OnAuthChanged;
            base.OnDestroy();
        }

        private void Start()
        {
            if (autoSignInOnStart)
                PlayGamesAuthService.SignIn(success =>
                {
                    StatusMessage = success ? "Connected to Google Play Games" : "Offline mode";
                    OnLeaderboardUpdated?.Invoke();
                });
        }

        private void OnDayAdvanced()
        {
            if (autoSubmitOnDayAdvance)
                SubmitScore(SaveManager.Instance.CurrentSave.totalLifetimeProfit);
        }

        private void OnAuthChanged(bool authenticated)
        {
            StatusMessage = authenticated ? "Signed in to Google Play Games" : "Not signed in";
            if (authenticated) RefreshOnlineScores();
            OnLeaderboardUpdated?.Invoke();
        }

        public void SignIn(Action<bool> callback = null) => PlayGamesAuthService.SignIn(callback);

        public void SubmitScore(double totalProfit)
        {
            var save = SaveManager.Instance.CurrentSave;
            UpsertLocalEntry(save.playerName, totalProfit);

            long scoreValue = (long)Math.Max(0, Math.Min(totalProfit, long.MaxValue));
            PlayGamesLeaderboardService.SubmitScore(primaryLeaderboardId, scoreValue, success =>
            {
                StatusMessage = success ? "Score submitted!" : "Score submit failed";
                if (success) RefreshOnlineScores();
                OnLeaderboardUpdated?.Invoke();
            });

            Debug.Log($"[Leaderboard] Submitted score: {totalProfit}");
        }

        public void RefreshOnlineScores()
        {
            if (!PlayGamesAuthService.IsAuthenticated)
            {
                StatusMessage = "Sign in to view global rankings";
                OnLeaderboardUpdated?.Invoke();
                return;
            }

            IsLoadingOnline = true;
            StatusMessage = "Loading global scores…";
            OnLeaderboardUpdated?.Invoke();

            PlayGamesLeaderboardService.LoadTopScores(primaryLeaderboardId, 25, entries =>
            {
                _onlineEntries.Clear();
                _onlineEntries.AddRange(entries);
                IsLoadingOnline = false;
                StatusMessage = entries.Count > 0
                    ? $"Global rankings ({entries.Count} players)"
                    : "No online scores yet";
                OnLeaderboardUpdated?.Invoke();
            });
        }

        public void ShowNativeLeaderboard() =>
            PlayGamesLeaderboardService.ShowNativeLeaderboard(primaryLeaderboardId);

        /// <summary>Returns online scores when available, otherwise local cache.</summary>
        public IReadOnlyList<LeaderboardEntry> GetDisplayLeaderboard()
        {
            if (_onlineEntries.Count > 0)
            {
                var display = new List<LeaderboardEntry>();
                foreach (var e in _onlineEntries)
                {
                    display.Add(new LeaderboardEntry
                    {
                        rank = e.rank,
                        playerName = e.isLocalPlayer ? PlayGamesAuthService.PlayerDisplayName : $"Player {e.rank}",
                        score = e.score,
                        isOnline = true
                    });
                }
                return display;
            }

            _localEntries.Sort((a, b) => b.score.CompareTo(a.score));
            for (int i = 0; i < _localEntries.Count; i++)
            {
                var entry = _localEntries[i];
                entry.rank = i + 1;
                _localEntries[i] = entry;
            }
            return _localEntries;
        }

        public IReadOnlyList<LeaderboardEntry> GetLocalLeaderboard() => GetDisplayLeaderboard();

        private void UpsertLocalEntry(string name, double score)
        {
            int index = _localEntries.FindIndex(e => e.playerName == name);
            if (index >= 0)
            {
                var entry = _localEntries[index];
                entry.score = Math.Max(entry.score, score);
                _localEntries[index] = entry;
            }
            else
            {
                _localEntries.Add(new LeaderboardEntry { playerName = name, score = score });
                if (_localEntries.Count > maxLocalEntries)
                    _localEntries.RemoveAt(_localEntries.Count - 1);
            }
        }
    }
}
