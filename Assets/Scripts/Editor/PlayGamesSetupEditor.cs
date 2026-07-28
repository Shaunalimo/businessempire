#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace BusinessEmpire.Editor
{
    /// <summary>
    /// Google Play Games setup instructions and leaderboard ID helper.
    /// </summary>
    public static class PlayGamesSetupEditor
    {
        [MenuItem("Business Empire/Install Google Play Games (Instructions)")]
        public static void ShowInstructions()
        {
            EditorUtility.DisplayDialog("Google Play Games Setup",
                "1. Install Play Games Plugin for Unity:\n" +
                "   Window → Google Play Games → Setup → Android\n\n" +
                "2. Link your game in Google Play Console → Play Games Services\n\n" +
                "3. Create leaderboards and copy IDs into:\n" +
                "   Assets/Scripts/Core/PlayGamesConstants.cs\n\n" +
                "4. Run: Business Empire → Setup Monetization SDKs\n\n" +
                "5. Enable Google Play App Signing on your release build\n\n" +
                "Leaderboard IDs format: CgkIxxxxxxxxEAIQAQ",
                "OK");
        }

        [MenuItem("Business Empire/Open Play Games Constants")]
        public static void OpenConstants()
        {
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(
                "Assets/Scripts/Core/PlayGamesConstants.cs");
            if (asset != null) AssetDatabase.OpenAsset(asset);
        }
    }
}
#endif
