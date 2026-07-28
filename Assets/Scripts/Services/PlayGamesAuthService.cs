using System;
using BusinessEmpire.Managers;
using UnityEngine;

namespace BusinessEmpire.Services
{
    /// <summary>
    /// Google Play Games authentication wrapper.
    /// Requires GOOGLE_PLAY_GAMES define and Play Games Plugin for Unity.
    /// </summary>
    public static class PlayGamesAuthService
    {
        public static bool IsAuthenticated { get; private set; }
        public static string PlayerDisplayName { get; private set; } = "CEO";
        public static string PlayerId { get; private set; }

        public static event Action<bool> OnAuthStateChanged;

        public static void Initialize()
        {
#if GOOGLE_PLAY_GAMES && UNITY_ANDROID
            try
            {
                GooglePlayGames.PlayGamesPlatform.Activate();
                Debug.Log("[PlayGames] Platform activated");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[PlayGames] Init failed: {ex.Message}");
            }
#else
            Debug.Log("[PlayGames] Simulation mode — install Play Games Plugin for production");
#endif
        }

        public static void SignIn(Action<bool> callback = null)
        {
#if GOOGLE_PLAY_GAMES && UNITY_ANDROID
            Social.localUser.Authenticate(success =>
            {
                IsAuthenticated = success;
                if (success)
                {
                    PlayerDisplayName = Social.localUser.userName;
                    PlayerId = Social.localUser.id;

                    if (SaveManager.Instance?.CurrentSave != null &&
                        (string.IsNullOrEmpty(SaveManager.Instance.CurrentSave.playerName) ||
                         SaveManager.Instance.CurrentSave.playerName == "CEO"))
                    {
                        SaveManager.Instance.CurrentSave.playerName = PlayerDisplayName;
                    }
                }
                Debug.Log(success ? $"[PlayGames] Signed in as {PlayerDisplayName}" : "[PlayGames] Sign-in failed");
                OnAuthStateChanged?.Invoke(success);
                callback?.Invoke(success);
            });
#else
            IsAuthenticated = true;
            PlayerDisplayName = "Local CEO";
            PlayerId = "local";
            OnAuthStateChanged?.Invoke(true);
            callback?.Invoke(true);
#endif
        }

        public static void SignOut()
        {
            IsAuthenticated = false;
            PlayerDisplayName = "CEO";
            PlayerId = null;
            OnAuthStateChanged?.Invoke(false);
        }
    }
}
