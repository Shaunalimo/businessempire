#if UNITY_EDITOR
using System.Linq;
using BusinessEmpire.UI;
using TMPro;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.Editor
{
    /// <summary>
    /// Configures monetization scripting defines and test UI.
    /// Menu: Business Empire → Setup Monetization SDKs
    /// </summary>
    [InitializeOnLoad]
    public static class MonetizationSetupEditor
    {
        private const string AdMobDefine = "GOOGLE_MOBILE_ADS";
        private const string PlayGamesDefine = "GOOGLE_PLAY_GAMES";
        private const string PrototypeScene = "Assets/Scenes/UIMobilePrototype.unity";

        static MonetizationSetupEditor()
        {
            EditorApplication.delayCall += SyncScriptingDefines;
        }

        [MenuItem("Business Empire/Setup Monetization SDKs")]
        public static void SetupMonetization()
        {
            SyncScriptingDefines();
            EnsureMonetizationTestPanel();
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Monetization Setup",
                "Monetization structure configured.\n\n" +
                "• TEST ad IDs in MonetizationConstants.cs\n" +
                "• Docs: Assets/Documentation/MONETIZATION.md\n" +
                "• Test panel added to UIMobilePrototype (if scene exists)\n\n" +
                "Import Google Mobile Ads SDK, then run this menu again to enable GOOGLE_MOBILE_ADS.",
                "OK");
        }

        public static void SyncScriptingDefines()
        {
            bool hasAdMob = HasAssembly("GoogleMobileAds");
            bool hasPlayGames = HasAssembly("GooglePlayGames");

            SetDefine(AdMobDefine, hasAdMob);
            SetDefine(PlayGamesDefine, hasPlayGames);

            Debug.Log(hasAdMob
                ? "[Monetization] GOOGLE_MOBILE_ADS enabled."
                : "[Monetization] Ads run in simulation mode (TEST IDs). Import Google Mobile Ads SDK for devices.");

            if (hasPlayGames)
                Debug.Log("[Monetization] GOOGLE_PLAY_GAMES enabled.");
        }

        [MenuItem("Business Empire/Install Google Mobile Ads (Instructions)")]
        public static void ShowAdMobInstallInstructions()
        {
            EditorUtility.DisplayDialog("Google Mobile Ads Setup",
                "1. Download: https://github.com/googleads/googleads-mobile-unity/releases\n\n" +
                "2. Import the .unitypackage\n\n" +
                "3. Run: Business Empire → Setup Monetization SDKs\n\n" +
                "4. Replace TEST IDs in:\n   Assets/Scripts/Monetization/MonetizationConstants.cs\n\n" +
                "5. See: Assets/Documentation/MONETIZATION.md",
                "OK");
        }

        [MenuItem("Business Empire/Open Monetization Documentation")]
        public static void OpenDocs()
        {
            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Documentation/MONETIZATION.md");
            if (asset != null) AssetDatabase.OpenAsset(asset);
        }

        public static void EnsureMonetizationTestPanel()
        {
            if (!System.IO.File.Exists(PrototypeScene)) return;

            var scene = EditorSceneManager.OpenScene(PrototypeScene);
            if (GameObject.Find("MonetizationTestPanel") != null)
            {
                EditorSceneManager.SaveScene(scene);
                return;
            }

            var canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null) return;

            var panel = UIFactory.CreatePopupShell(canvas.transform, "MonetizationTestPanel", out var anim, out var close);
            panel.GetComponent<RectTransform>().anchorMin = new Vector2(0.05f, 0.05f);
            panel.GetComponent<RectTransform>().anchorMax = new Vector2(0.95f, 0.55f);

            var root = panel.transform.Find("Panel");
            var status = UIFactory.CreateText(root, "Status", "Monetization test (TEST IDs)", 24, UITheme.TextMuted,
                TextAlignmentOptions.Top);
            status.rectTransform.sizeDelta = new Vector2(900, 80);

            var grid = new GameObject("Buttons");
            grid.transform.SetParent(root, false);
            var glg = grid.AddComponent<GridLayoutGroup>();
            glg.cellSize = new Vector2(400, 72);
            glg.spacing = new Vector2(12, 12);
            glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            glg.constraintCount = 2;

            var rewarded = UIFactory.CreateButton(grid.transform, "Rewarded", "Test Rewarded", UITheme.AccentPrimary, 72);
            var inter = UIFactory.CreateButton(grid.transform, "Inter", "Test Interstitial", UITheme.AccentSecondary, 72);
            var banner = UIFactory.CreateButton(grid.transform, "Banner", "Toggle Banner", UITheme.AccentWarning, 72);
            var remove = UIFactory.CreateButton(grid.transform, "Remove", "Buy Remove Ads", UITheme.AccentSuccess, 72);
            var gems = UIFactory.CreateButton(grid.transform, "Gems", "Buy 100 Gems", UITheme.AccentPremium, 72);
            var starter = UIFactory.CreateButton(grid.transform, "Starter", "Buy Starter Pack", UITheme.AccentPrimary, 72);
            var cash = UIFactory.CreateButton(grid.transform, "Cash", "Rewarded: Extra Cash", UITheme.BackgroundElevated, 72);
            var offline = UIFactory.CreateButton(grid.transform, "Offline", "Rewarded: 2x Offline", UITheme.BackgroundElevated, 72);
            var daily = UIFactory.CreateButton(grid.transform, "Daily", "Rewarded: Daily x2", UITheme.BackgroundElevated, 72);

            var test = panel.AddComponent<MonetizationTestPanel>();
            UIFactory.SetSerializedField(test, "statusText", status);
            UIFactory.SetSerializedField(test, "rewardedButton", rewarded);
            UIFactory.SetSerializedField(test, "interstitialButton", inter);
            UIFactory.SetSerializedField(test, "bannerToggleButton", banner);
            UIFactory.SetSerializedField(test, "removeAdsButton", remove);
            UIFactory.SetSerializedField(test, "gemsButton", gems);
            UIFactory.SetSerializedField(test, "starterPackButton", starter);
            UIFactory.SetSerializedField(test, "extraCashRewardButton", cash);
            UIFactory.SetSerializedField(test, "doubleOfflineButton", offline);
            UIFactory.SetSerializedField(test, "dailyMultiplierButton", daily);
            UIFactory.SetSerializedField(test, "closeButton", close);

            panel.SetActive(false);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[Monetization] Test panel added to UIMobilePrototype scene.");
        }

        private static bool HasAssembly(string assemblyName) =>
            System.AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == assemblyName);

        private static void SetDefine(string define, bool enabled)
        {
            var targets = new[]
            {
                NamedBuildTarget.FromBuildTargetGroup(BuildTargetGroup.Android),
                NamedBuildTarget.FromBuildTargetGroup(BuildTargetGroup.iOS)
            };

            foreach (var target in targets)
            {
                var defines = PlayerSettings.GetScriptingDefineSymbols(target)
                    .Split(';').Where(d => !string.IsNullOrEmpty(d)).ToList();
                if (enabled && !defines.Contains(define)) defines.Add(define);
                else if (!enabled) defines.Remove(define);
                PlayerSettings.SetScriptingDefineSymbols(target, string.Join(";", defines));
            }
        }
    }
}
#endif
