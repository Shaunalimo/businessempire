using System;
using System.Collections.Generic;
using BusinessEmpire.Core;
using UnityEngine;

#if GOOGLE_PLAY_GAMES
using GooglePlayGames;
#endif

namespace BusinessEmpire.Services
{
    /// <summary>
    /// Google Play Games leaderboard read/write operations.
    /// </summary>
    public static class PlayGamesLeaderboardService
    {
        public struct OnlineEntry
        {
            public int rank;
            public string playerName;
            public long score;
            public bool isLocalPlayer;
        }

        public static bool IsAvailable
        {
            get
            {
#if GOOGLE_PLAY_GAMES && UNITY_ANDROID
                return PlayGamesAuthService.IsAuthenticated;
#else
                return false;
#endif
            }
        }

        public static void SubmitScore(string leaderboardId, long score, Action<bool> callback = null)
        {
#if GOOGLE_PLAY_GAMES && UNITY_ANDROID
            if (!PlayGamesAuthService.IsAuthenticated)
            {
                callback?.Invoke(false);
                return;
            }

            Social.ReportScore(score, leaderboardId, success =>
            {
                Debug.Log(success
                    ? $"[PlayGames] Score submitted: {score}"
                    : "[PlayGames] Score submission failed");
                callback?.Invoke(success);
            });
#else
            Debug.Log($"[PlayGames] Simulated score submit: {score}");
            callback?.Invoke(true);
#endif
        }

        public static void ShowNativeLeaderboard(string leaderboardId = null)
        {
#if GOOGLE_PLAY_GAMES && UNITY_ANDROID
            if (!PlayGamesAuthService.IsAuthenticated)
            {
                PlayGamesAuthService.SignIn(_ => ShowNativeLeaderboard(leaderboardId));
                return;
            }

            if (string.IsNullOrEmpty(leaderboardId))
                PlayGamesPlatform.Instance.ShowLeaderboardUI();
            else
                PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderboardId);
#else
            Debug.Log("[PlayGames] Native leaderboard UI requires Play Games Plugin");
#endif
        }

        public static void LoadTopScores(string leaderboardId, int count, Action<List<OnlineEntry>> callback)
        {
            var results = new List<OnlineEntry>();

#if GOOGLE_PLAY_GAMES && UNITY_ANDROID
            if (!PlayGamesAuthService.IsAuthenticated)
            {
                callback?.Invoke(results);
                return;
            }

            PlayGamesPlatform.Instance.LoadScores(
                leaderboardId,
                LeaderboardStart.PlayerCentered,
                count,
                (data) =>
                {
                    if (data == null)
                    {
                        callback?.Invoke(results);
                        return;
                    }

                    int rank = 1;
                    foreach (var score in data.Scores)
                    {
                        int displayRank = rank;
                        bool isLocal = score.userID == PlayGamesAuthService.PlayerId;
                        results.Add(new OnlineEntry
                        {
                            rank = displayRank,
                            playerName = isLocal
                                ? PlayGamesAuthService.PlayerDisplayName
                                : $"Rival #{displayRank}",
                            score = score.value,
                            isLocalPlayer = isLocal
                        });
                        rank++;
                    }
                    callback?.Invoke(results);
                });
#else
            callback?.Invoke(results);
#endif
        }
    }
}
