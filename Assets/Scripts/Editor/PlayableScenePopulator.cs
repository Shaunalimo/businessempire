#if UNITY_EDITOR
using System.IO;
using BusinessEmpire.Core;
using BusinessEmpire.Managers;
using BusinessEmpire.Monetization;
using BusinessEmpire.UI;
using BusinessEmpire.UI.Runtime;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BusinessEmpire.Editor
{
    /// <summary>
    /// Populates MainMenu, Gameplay, and Bootstrap scenes with full saved UI hierarchies.
    /// Menu: Business Empire → Populate Playable Scenes (also runs via ProjectAutoSetup on editor load).
    /// </summary>
    public static class PlayableScenePopulator
    {
        private const string BootstrapPath = "Assets/Scenes/Bootstrap.unity";
        private const string MainMenuPath = "Assets/Scenes/MainMenu.unity";
        private const string GameplayPath = "Assets/Scenes/Gameplay.unity";

        [MenuItem("Business Empire/Populate Playable Scenes")]
        public static void PopulateAll()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogWarning("[Business Empire] Cannot populate scenes while in Play mode.");
                return;
            }

            ImportTmpEssentials();
            PopulateBootstrapScene();
            PopulateMainMenuScene();
            PopulateGameplayScene();
            ConfigureBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Business Empire] Playable scenes populated. Open MainMenu.unity or Gameplay.unity — UI is visible in the Hierarchy.");
        }

        public static bool ScenesNeedPopulation()
        {
            return SceneNeedsPopulation(MainMenuPath)
                   || SceneNeedsPopulation(GameplayPath)
                   || SceneNeedsPopulation(BootstrapPath);
        }

        private static bool SceneNeedsPopulation(string path)
        {
            if (!File.Exists(path))
                return true;

            var text = File.ReadAllText(path);
            if (text.Contains("PlayableMainMenu") || text.Contains("PlayableGameplay"))
                return true;

            return !text.Contains("m_Name: Canvas");
        }

        private static void PopulateBootstrapScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            new GameObject("BootstrapManager").AddComponent<BootstrapManager>();
            new GameObject("SceneLoader").AddComponent<SceneLoader>();

            EditorSceneManager.SaveScene(scene, BootstrapPath);
        }

        private static void PopulateMainMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            EnsureEventSystem();

            var canvas = UIFactory.CreateCanvas("Canvas");
            UIFactory.CreatePanel(canvas.transform, "BackgroundPanel", UITheme.BackgroundDark);

            var accent = UIFactory.CreatePanel(canvas.transform, "AccentBar", UITheme.AccentPrimary, false);
            var accentRt = accent.GetComponent<RectTransform>();
            accentRt.anchorMin = new Vector2(0, 1);
            accentRt.anchorMax = new Vector2(1, 1);
            accentRt.sizeDelta = new Vector2(0, 8);

            var title = UIFactory.CreateText(canvas.transform, "Title", "AI Business Tycoon", 56, UITheme.TextPrimary);
            title.rectTransform.anchorMin = new Vector2(0, 0.72f);
            title.rectTransform.anchorMax = new Vector2(1, 0.82f);

            var subtitle = UIFactory.CreateText(canvas.transform, "Subtitle", "Build your global business empire", 36,
                UITheme.AccentPrimary);
            subtitle.rectTransform.anchorMin = new Vector2(0, 0.64f);
            subtitle.rectTransform.anchorMax = new Vector2(1, 0.72f);

            var btnArea = new GameObject("Buttons");
            btnArea.transform.SetParent(canvas.transform, false);
            var btnRt = btnArea.AddComponent<RectTransform>();
            btnRt.anchorMin = new Vector2(0.15f, 0.22f);
            btnRt.anchorMax = new Vector2(0.85f, 0.55f);
            var vlg = btnArea.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 16;
            vlg.childControlHeight = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth = true;
            vlg.childForceExpandWidth = true;

            var newGameBtn = UIFactory.CreateButton(btnArea.transform, "NewGameButton", "New Game", UITheme.AccentSuccess);
            var continueBtn = UIFactory.CreateButton(btnArea.transform, "ContinueButton", "Continue", UITheme.AccentPrimary);
            var settingsBtn = UIFactory.CreateButton(btnArea.transform, "SettingsButton", "Settings", UITheme.BackgroundElevated);
            var exitBtn = UIFactory.CreateButton(btnArea.transform, "ExitButton", "Exit", UITheme.AccentDanger, 80);

            var version = UIFactory.CreateText(canvas.transform, "VersionText", "v0.1 Prototype", 22, UITheme.TextMuted);
            version.rectTransform.anchorMin = new Vector2(0, 0.02f);
            version.rectTransform.anchorMax = new Vector2(1, 0.08f);

            var settingsPanel = BuildSimpleSettingsPanel(canvas.transform, out var settingsCloseBtn);

            var controllerGo = new GameObject("MainMenuSceneController");
            controllerGo.transform.SetParent(canvas.transform, false);
            var controller = controllerGo.AddComponent<MainMenuSceneController>();
            WireMainMenuController(controller, newGameBtn, continueBtn, settingsBtn, exitBtn, settingsCloseBtn,
                settingsPanel, version);

            new GameObject("UIManager").AddComponent<UIManager>();
            new GameObject("AudioManager").AddComponent<AudioManager>();

            EditorSceneManager.SaveScene(scene, MainMenuPath);
        }

        private static GameObject BuildSimpleSettingsPanel(Transform canvas, out Button closeButton)
        {
            var panel = UIFactory.CreatePanel(canvas, "SettingsPanel", new Color(0, 0, 0, 0.85f));
            panel.SetActive(false);

            var box = UIFactory.CreatePanel(panel.transform, "Box", UITheme.BackgroundCard, false);
            var boxRt = box.GetComponent<RectTransform>();
            boxRt.anchorMin = new Vector2(0.1f, 0.25f);
            boxRt.anchorMax = new Vector2(0.9f, 0.75f);

            UIFactory.CreateText(box.transform, "Header", "Settings", 44, UITheme.TextPrimary).rectTransform
                .anchoredPosition = new Vector2(0, 200);

            closeButton = UIFactory.CreateButton(box.transform, "CloseButton", "Close", UITheme.AccentPrimary, 72);
            closeButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -180);

            return panel;
        }

        private static void WireMainMenuController(MainMenuSceneController controller, Button newGame,
            Button continueBtn, Button settings, Button exit, Button settingsClose, GameObject settingsPanel,
            TextMeshProUGUI version)
        {
            UIFactory.SetSerializedField(controller, "newGameButton", newGame);
            UIFactory.SetSerializedField(controller, "continueButton", continueBtn);
            UIFactory.SetSerializedField(controller, "settingsButton", settings);
            UIFactory.SetSerializedField(controller, "exitButton", exit);
            UIFactory.SetSerializedField(controller, "settingsCloseButton", settingsClose);
            UIFactory.SetSerializedField(controller, "settingsPanel", settingsPanel);
            UIFactory.SetSerializedField(controller, "versionText", version);
        }

        private static void PopulateGameplayScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            EnsureEventSystem();

            var canvas = UIFactory.CreateCanvas("Canvas");
            var build = PlayableUILayoutBuilder.BuildGameplayInterface(canvas.transform);

            var controllerGo = new GameObject("GameplaySceneController");
            var controller = controllerGo.AddComponent<GameplaySceneController>();
            UIFactory.SetSerializedField(controller, "buyBusinessButton", build.BuyBusinessButton);
            UIFactory.SetSerializedField(controller, "upgradeBusinessButton", build.UpgradeBusinessButton);
            UIFactory.SetSerializedField(controller, "earnMoneyButton", build.EarnMoneyButton);

            new GameObject("GameManager").AddComponent<GameManager>();
            new GameObject("EconomyManager").AddComponent<EconomyManager>();
            new GameObject("SaveLoadManager").AddComponent<SaveLoadManager>();
            new GameObject("MonetizationManager").AddComponent<MonetizationManager>();
            new GameObject("UIManager").AddComponent<UIManager>();

            EditorSceneManager.SaveScene(scene, GameplayPath);
        }

        private static void ConfigureBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(BootstrapPath, true),
                new EditorBuildSettingsScene(MainMenuPath, true),
                new EditorBuildSettingsScene(GameplayPath, true)
            };
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<EventSystem>() != null)
                return;

            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        private static void ImportTmpEssentials()
        {
            AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                "Packages/com.unity.textmeshpro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        }
    }
}
#endif
