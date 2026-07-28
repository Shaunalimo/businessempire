#if UNITY_EDITOR
using BusinessEmpire.Core;
using BusinessEmpire.Data;
using BusinessEmpire.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BusinessEmpire.Editor
{
    /// <summary>
    /// Builds complete UI prefabs and wired scenes for Business Empire.
    /// Menu: Business Empire > Build Complete UI
    /// </summary>
    public static class GameUIBuilder
    {
        private const string PrefabPath = "Assets/Prefabs/UI";
        private const string ResourcePath = "Assets/Resources";
        private const string BootstrapPath = "Assets/Scenes/Bootstrap.unity";
        private const string MainMenuPath = "Assets/Scenes/MainMenu.unity";
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("Business Empire/Build Complete UI")]
        public static void BuildAll()
        {
            EnsureFolders();
            ImportTmpEssentials();
            SfxGeneratorEditor.GenerateAll();
            BusinessAssetGenerator.GenerateAll();
            var catalog = CreateIapCatalogAsset();

            // Full 20-screen mobile UI prototype
            CompleteMobileUIBuilder.Build();

            // Legacy scenes + prefabs (optional game integration)
            var cardPrefab = BuildBusinessCardPrefab();
            var optionPrefab = BuildFinanceOptionPrefab();
            var shopPrefab = BuildShopProductPrefab();
            var entryPrefab = BuildLeaderboardEntryPrefab();
            BuildWorldPrefabs();
            BuildBootstrapScene();
            BuildMainMenuScene(catalog);
            BuildGameScene(cardPrefab, optionPrefab, shopPrefab, entryPrefab, catalog);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Business Empire] Complete UI built! Open UIMobilePrototype scene to preview all 20 screens.");
        }

        private static void EnsureFolders()
        {
            CreateFolder("Assets/Scenes");
            CreateFolder("Assets/Prefabs");
            CreateFolder(PrefabPath);
            CreateFolder(ResourcePath);
        }

        private static void CreateFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parent = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
            var name = System.IO.Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, name);
        }

        private static void ImportTmpEssentials()
        {
            // Triggers TMP resource import if missing
            AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                "Packages/com.unity.textmeshpro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        }

        private static IapProductCatalog CreateIapCatalogAsset()
        {
            string path = $"{ResourcePath}/IapProductCatalog.asset";
            var existing = AssetDatabase.LoadAssetAtPath<IapProductCatalog>(path);
            if (existing != null) return existing;

            var catalog = ScriptableObject.CreateInstance<IapProductCatalog>();
            AssetDatabase.CreateAsset(catalog, path);
            return catalog;
        }

        #region Prefabs

        private static BusinessCardUI BuildBusinessCardPrefab()
        {
            var root = UIFactory.CreatePanel(null, "BusinessCard", UITheme.BackgroundElevated, false);
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(900, 140);
            var layout = root.AddComponent<LayoutElement>();
            layout.minHeight = 140;
            layout.preferredHeight = 140;

            var icon = UIFactory.CreatePanel(root.transform, "Icon", UITheme.AccentPrimary, false);
            icon.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
            icon.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(96, 96);
            icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(60, 0);

            var nameText = UIFactory.CreateText(root.transform, "Name", "Business", 36, UITheme.TextPrimary,
                TextAlignmentOptions.Left);
            nameText.rectTransform.anchorMin = new Vector2(0, 0.55f);
            nameText.rectTransform.anchorMax = new Vector2(1, 1);
            nameText.rectTransform.offsetMin = new Vector2(120, 0);
            nameText.rectTransform.offsetMax = new Vector2(-120, -8);

            var statusText = UIFactory.CreateText(root.transform, "Status", "Status", 26, UITheme.TextSecondary,
                TextAlignmentOptions.Left);
            statusText.rectTransform.anchorMin = new Vector2(0, 0.1f);
            statusText.rectTransform.anchorMax = new Vector2(0.65f, 0.55f);
            statusText.rectTransform.offsetMin = new Vector2(120, 0);

            var profitText = UIFactory.CreateText(root.transform, "Profit", "$0", 30, UITheme.AccentSuccess,
                TextAlignmentOptions.Right);
            profitText.rectTransform.anchorMin = new Vector2(0.55f, 0.1f);
            profitText.rectTransform.anchorMax = new Vector2(1, 0.55f);
            profitText.rectTransform.offsetMax = new Vector2(-16, 0);

            var btn = UIFactory.CreateButton(root.transform, "ActionButton", "Open", UITheme.AccentPrimary, 72);
            btn.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
            btn.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
            btn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, 0);
            btn.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 72);

            var card = root.AddComponent<BusinessCardUI>();
            UIFactory.SetSerializedField(card, "iconImage", icon.GetComponent<Image>());
            UIFactory.SetSerializedField(card, "nameText", nameText);
            UIFactory.SetSerializedField(card, "statusText", statusText);
            UIFactory.SetSerializedField(card, "profitText", profitText);
            UIFactory.SetSerializedField(card, "actionButton", btn);

            return SavePrefab(root, "BusinessCardUI").GetComponent<BusinessCardUI>();
        }

        private static FinanceOptionUI BuildFinanceOptionPrefab()
        {
            var root = UIFactory.CreatePanel(null, "FinanceOption", UITheme.BackgroundElevated, false);
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(900, 120);
            root.AddComponent<LayoutElement>().minHeight = 120;

            var title = UIFactory.CreateText(root.transform, "Title", "Option", 32, UITheme.TextPrimary,
                TextAlignmentOptions.Left);
            title.rectTransform.anchorMin = new Vector2(0, 0.5f);
            title.rectTransform.anchorMax = new Vector2(0.55f, 1);
            title.rectTransform.offsetMin = new Vector2(20, 0);

            var detail = UIFactory.CreateText(root.transform, "Detail", "Details", 24, UITheme.TextSecondary,
                TextAlignmentOptions.Left);
            detail.rectTransform.anchorMin = new Vector2(0, 0);
            detail.rectTransform.anchorMax = new Vector2(0.55f, 0.5f);
            detail.rectTransform.offsetMin = new Vector2(20, 0);

            var cost = UIFactory.CreateText(root.transform, "Cost", "$0", 28, UITheme.AccentWarning,
                TextAlignmentOptions.Right);
            cost.rectTransform.anchorMin = new Vector2(0.55f, 0.5f);
            cost.rectTransform.anchorMax = new Vector2(0.85f, 1);

            var btn = UIFactory.CreateButton(root.transform, "Action", "Go", UITheme.AccentPrimary, 72);
            btn.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
            btn.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
            btn.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 72);
            btn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, 0);

            var ui = root.AddComponent<FinanceOptionUI>();
            UIFactory.SetSerializedField(ui, "titleText", title);
            UIFactory.SetSerializedField(ui, "costText", cost);
            UIFactory.SetSerializedField(ui, "detailText", detail);
            UIFactory.SetSerializedField(ui, "actionButton", btn);
            return SavePrefab(root, "FinanceOptionUI").GetComponent<FinanceOptionUI>();
        }

        private static ShopProductUI BuildShopProductPrefab()
        {
            var root = UIFactory.CreatePanel(null, "ShopProduct", UITheme.BackgroundElevated, false);
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(900, 130);
            root.AddComponent<LayoutElement>().minHeight = 130;

            var accent = UIFactory.CreatePanel(root.transform, "Accent", UITheme.AccentPremium, false);
            accent.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            accent.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            accent.GetComponent<RectTransform>().sizeDelta = new Vector2(8, 0);
            accent.GetComponent<RectTransform>().anchoredPosition = new Vector2(4, 0);

            var title = UIFactory.CreateText(root.transform, "Title", "Product", 34, UITheme.TextPrimary,
                TextAlignmentOptions.Left);
            title.rectTransform.anchorMin = new Vector2(0, 0.55f);
            title.rectTransform.anchorMax = new Vector2(0.7f, 1);
            title.rectTransform.offsetMin = new Vector2(24, 0);

            var desc = UIFactory.CreateText(root.transform, "Desc", "Description", 24, UITheme.TextSecondary,
                TextAlignmentOptions.Left);
            desc.rectTransform.anchorMin = new Vector2(0, 0);
            desc.rectTransform.anchorMax = new Vector2(0.7f, 0.55f);
            desc.rectTransform.offsetMin = new Vector2(24, 0);

            var price = UIFactory.CreateText(root.transform, "Price", "$0.99", 30, UITheme.AccentPremium,
                TextAlignmentOptions.Right);
            price.rectTransform.anchorMin = new Vector2(0.65f, 0.55f);
            price.rectTransform.anchorMax = new Vector2(1, 1);
            price.rectTransform.offsetMax = new Vector2(-140, 0);

            var btn = UIFactory.CreateButton(root.transform, "Buy", "Buy", UITheme.AccentPremium, 72);
            btn.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
            btn.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
            btn.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 72);
            btn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, 0);

            var ui = root.AddComponent<ShopProductUI>();
            UIFactory.SetSerializedField(ui, "titleText", title);
            UIFactory.SetSerializedField(ui, "descriptionText", desc);
            UIFactory.SetSerializedField(ui, "priceText", price);
            UIFactory.SetSerializedField(ui, "buyButton", btn);
            UIFactory.SetSerializedField(ui, "accentBar", accent.GetComponent<Image>());
            return SavePrefab(root, "ShopProductUI").GetComponent<ShopProductUI>();
        }

        private static LeaderboardEntryUI BuildLeaderboardEntryPrefab()
        {
            var root = UIFactory.CreatePanel(null, "LeaderboardEntry", UITheme.BackgroundElevated, false);
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(900, 80);
            root.AddComponent<LayoutElement>().minHeight = 80;

            var rank = UIFactory.CreateText(root.transform, "Rank", "#1", 32, UITheme.AccentPrimary,
                TextAlignmentOptions.Left);
            rank.rectTransform.anchorMin = new Vector2(0, 0);
            rank.rectTransform.anchorMax = new Vector2(0.15f, 1);
            rank.rectTransform.offsetMin = new Vector2(16, 0);

            var name = UIFactory.CreateText(root.transform, "Name", "Player", 30, UITheme.TextPrimary,
                TextAlignmentOptions.Left);
            name.rectTransform.anchorMin = new Vector2(0.15f, 0);
            name.rectTransform.anchorMax = new Vector2(0.65f, 1);

            var score = UIFactory.CreateText(root.transform, "Score", "$0", 30, UITheme.AccentSuccess,
                TextAlignmentOptions.Right);
            score.rectTransform.anchorMin = new Vector2(0.65f, 0);
            score.rectTransform.anchorMax = new Vector2(1, 1);
            score.rectTransform.offsetMax = new Vector2(-16, 0);

            var ui = root.AddComponent<LeaderboardEntryUI>();
            UIFactory.SetSerializedField(ui, "rankText", rank);
            UIFactory.SetSerializedField(ui, "nameText", name);
            UIFactory.SetSerializedField(ui, "scoreText", score);
            return SavePrefab(root, "LeaderboardEntryUI").GetComponent<LeaderboardEntryUI>();
        }

        private static GameObject SavePrefab(GameObject temp, string name)
        {
            string path = name.Contains("/")
                ? (name.EndsWith(".prefab") ? name : $"{name}.prefab")
                : $"{PrefabPath}/{name}.prefab";
            var prefab = PrefabUtility.SaveAsPrefabAsset(temp, path);
            Object.DestroyImmediate(temp);
            return prefab;
        }

        #endregion

        #region Scenes

        private static void BuildBootstrapScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var bootstrap = new GameObject("GameBootstrap");
            bootstrap.AddComponent<GameBootstrap>();
            var init = new GameObject("AppInitializer");
            init.AddComponent<AppInitializer>();
            EditorSceneManager.SaveScene(scene, BootstrapPath);
        }

        private static void BuildMainMenuScene(IapProductCatalog catalog)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EnsureEventSystem();

            var canvas = UIFactory.CreateCanvas("Canvas");
            UIFactory.CreatePanel(canvas.transform, "Background", UITheme.BackgroundDark);

            // Decorative accent bar
            var accent = UIFactory.CreatePanel(canvas.transform, "AccentBar", UITheme.AccentPrimary, false);
            accent.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            accent.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            accent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 8);
            accent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            var title = UIFactory.CreateText(canvas.transform, "Title", "Business Empire", 72, UITheme.TextPrimary);
            title.rectTransform.anchorMin = new Vector2(0, 0.72f);
            title.rectTransform.anchorMax = new Vector2(1, 0.82f);

            var subtitle = UIFactory.CreateText(canvas.transform, "Subtitle", "CEO Simulator", 40, UITheme.AccentPrimary);
            subtitle.rectTransform.anchorMin = new Vector2(0, 0.66f);
            subtitle.rectTransform.anchorMax = new Vector2(1, 0.72f);

            var tagline = UIFactory.CreateText(canvas.transform, "Tagline", "Build your business empire", 28,
                UITheme.TextSecondary);
            tagline.rectTransform.anchorMin = new Vector2(0.1f, 0.58f);
            tagline.rectTransform.anchorMax = new Vector2(0.9f, 0.64f);

            // Buttons stack
            var btnArea = new GameObject("Buttons");
            btnArea.transform.SetParent(canvas.transform, false);
            var btnRt = btnArea.AddComponent<RectTransform>();
            btnRt.anchorMin = new Vector2(0.15f, 0.22f);
            btnRt.anchorMax = new Vector2(0.85f, 0.52f);
            var vlg = btnArea.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 20;
            vlg.childControlHeight = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth = true;
            vlg.childForceExpandWidth = true;

            var playBtn = UIFactory.CreateButton(btnArea.transform, "PlayButton", "▶  PLAY", UITheme.AccentSuccess);
            var shopBtn = UIFactory.CreateButton(btnArea.transform, "ShopButton", "💎  SHOP", UITheme.AccentPremium);
            var settingsBtn = UIFactory.CreateButton(btnArea.transform, "SettingsButton", "⚙  SETTINGS",
                UITheme.BackgroundElevated);
            var removeAdsBtn = UIFactory.CreateButton(btnArea.transform, "RemoveAdsButton", "Remove Ads",
                UITheme.AccentPrimary, 80);

            UIFactory.CreateText(canvas.transform, "Version", "v1.0.0", 22, UITheme.TextMuted).rectTransform
                .anchorMin = new Vector2(0, 0);
            var versionRt = canvas.transform.Find("Version").GetComponent<RectTransform>();
            versionRt.anchorMax = new Vector2(1, 0.05f);

            // Settings popup
            var settingsPanel = BuildSettingsPanel(canvas.transform, out _);

            // Shop popup
            var shopPanel = BuildShopPanel(canvas.transform, catalog, out _);

            // Main menu controller
            var menuGo = new GameObject("MainMenuUI");
            menuGo.transform.SetParent(canvas.transform, false);
            var menu = menuGo.AddComponent<MainMenuUI>();
            UIFactory.SetSerializedField(menu, "playButton", playBtn);
            UIFactory.SetSerializedField(menu, "shopButton", shopBtn);
            UIFactory.SetSerializedField(menu, "settingsButton", settingsBtn);
            UIFactory.SetSerializedField(menu, "removeAdsButton", removeAdsBtn);
            UIFactory.SetSerializedField(menu, "titleText", title);
            UIFactory.SetSerializedField(menu, "settingsPanel", settingsPanel);
            UIFactory.SetSerializedField(menu, "shopPanel", shopPanel);

            new GameObject("GameBootstrap").AddComponent<GameBootstrap>();
            EditorSceneManager.SaveScene(scene, MainMenuPath);
        }

        private static void BuildGameScene(BusinessCardUI cardPrefab, FinanceOptionUI optionPrefab,
            ShopProductUI shopPrefab, LeaderboardEntryUI entryPrefab, IapProductCatalog catalog)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            EnsureEventSystem();

            var canvas = UIFactory.CreateCanvas("Canvas");
            UIFactory.CreatePanel(canvas.transform, "Background", UITheme.BackgroundDark);

            // --- Top bar ---
            var topBar = UIFactory.CreatePanel(canvas.transform, "TopBar", UITheme.BackgroundCard, false);
            var topRt = topBar.GetComponent<RectTransform>();
            topRt.anchorMin = new Vector2(0, 1);
            topRt.anchorMax = new Vector2(1, 1);
            topRt.pivot = new Vector2(0.5f, 1);
            topRt.sizeDelta = new Vector2(0, UITheme.TopBarHeight);

            var cashText = UIFactory.CreateText(topBar.transform, "Cash", "$5,000", 34, UITheme.AccentSuccess,
                TextAlignmentOptions.Left);
            cashText.rectTransform.anchorMin = new Vector2(0, 0.55f);
            cashText.rectTransform.anchorMax = new Vector2(0.5f, 1);
            cashText.rectTransform.offsetMin = new Vector2(24, 0);

            var gemsText = UIFactory.CreateText(topBar.transform, "Gems", "10", 34, UITheme.AccentPremium,
                TextAlignmentOptions.Right);
            gemsText.rectTransform.anchorMin = new Vector2(0.5f, 0.55f);
            gemsText.rectTransform.anchorMax = new Vector2(1, 1);
            gemsText.rectTransform.offsetMax = new Vector2(-24, 0);

            var dayText = UIFactory.CreateText(topBar.transform, "Day", "Day 0", 28, UITheme.TextSecondary,
                TextAlignmentOptions.Left);
            dayText.rectTransform.anchorMin = new Vector2(0, 0);
            dayText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            dayText.rectTransform.offsetMin = new Vector2(24, 0);

            var sliderGo = new GameObject("DayProgress");
            sliderGo.transform.SetParent(topBar.transform, false);
            var sliderRt = sliderGo.AddComponent<RectTransform>();
            sliderRt.anchorMin = new Vector2(0.1f, 0.08f);
            sliderRt.anchorMax = new Vector2(0.9f, 0.18f);
            sliderRt.offsetMin = Vector2.zero;
            sliderRt.offsetMax = Vector2.zero;
            var slider = sliderGo.AddComponent<Slider>();
            slider.minValue = 0;
            slider.maxValue = 1;
            UIFactory.CreatePanel(sliderGo.transform, "Fill", UITheme.AccentPrimary);

            // --- Center content ---
            var center = UIFactory.CreateText(canvas.transform, "CenterTitle", "Your Empire", 52, UITheme.TextPrimary);
            center.rectTransform.anchorMin = new Vector2(0, 0.45f);
            center.rectTransform.anchorMax = new Vector2(1, 0.52f);

            var hint = UIFactory.CreateText(canvas.transform, "Hint", "Tap Businesses to expand your portfolio",
                26, UITheme.TextSecondary);
            hint.rectTransform.anchorMin = new Vector2(0.08f, 0.38f);
            hint.rectTransform.anchorMax = new Vector2(0.92f, 0.44f);

            // --- Bottom nav ---
            var bottomNav = UIFactory.CreatePanel(canvas.transform, "BottomNav", UITheme.BackgroundCard, false);
            var navRt = bottomNav.GetComponent<RectTransform>();
            navRt.anchorMin = new Vector2(0, 0);
            navRt.anchorMax = new Vector2(1, 0);
            navRt.pivot = new Vector2(0.5f, 0);
            navRt.sizeDelta = new Vector2(0, UITheme.BottomNavHeight);

            var navGrid = bottomNav.AddComponent<GridLayoutGroup>();
            navGrid.cellSize = new Vector2(160, 120);
            navGrid.spacing = new Vector2(8, 8);
            navGrid.padding = new RectOffset(24, 24, 16, 16);
            navGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            navGrid.constraintCount = 3;

            var bizBtn = UIFactory.CreateButton(bottomNav.transform, "Biz", "Business", UITheme.BackgroundElevated, 100);
            var finBtn = UIFactory.CreateButton(bottomNav.transform, "Finance", "Finance", UITheme.BackgroundElevated, 100);
            var mktBtn = UIFactory.CreateButton(bottomNav.transform, "Marketing", "Market", UITheme.BackgroundElevated, 100);
            var dailyBtn = UIFactory.CreateButton(bottomNav.transform, "Daily", "Daily", UITheme.AccentWarning, 100);
            var shopBtn = UIFactory.CreateButton(bottomNav.transform, "Shop", "Shop", UITheme.AccentPremium, 100);
            var setBtn = UIFactory.CreateButton(bottomNav.transform, "Settings", "More", UITheme.BackgroundElevated, 100);

            // --- Panels ---
            var panelsRoot = new GameObject("Panels");
            panelsRoot.transform.SetParent(canvas.transform, false);
            UIFactory.StretchFull(panelsRoot.AddComponent<RectTransform>());

            var businessPanel = BuildBusinessListPanel(panelsRoot.transform, cardPrefab, out var detailPanel);
            var financePanel = BuildFinancePanel(panelsRoot.transform, optionPrefab);
            var marketingPanel = BuildMarketingPanel(panelsRoot.transform, optionPrefab);
            var dailyPanel = BuildDailyRewardPanel(panelsRoot.transform);
            var leaderboardPanel = BuildLeaderboardPanel(panelsRoot.transform, entryPrefab);
            var settingsPanel = BuildSettingsPanel(panelsRoot.transform, out _, leaderboardPanel);
            var shopPanel = BuildShopPanel(panelsRoot.transform, catalog, out _);
            var profitPanel = BuildProfitReportPanel(panelsRoot.transform);
            var offlinePopup = BuildOfflineIncomePopup(panelsRoot.transform);
            var eventToast = BuildEventNotification(panelsRoot.transform);

            // --- Game HUD ---
            var hudGo = new GameObject("GameHUD");
            hudGo.transform.SetParent(canvas.transform, false);
            var hud = hudGo.AddComponent<GameHUD>();
            UIFactory.SetSerializedField(hud, "cashText", cashText);
            UIFactory.SetSerializedField(hud, "gemsText", gemsText);
            UIFactory.SetSerializedField(hud, "dayText", dayText);
            UIFactory.SetSerializedField(hud, "dayProgressSlider", slider);
            UIFactory.SetSerializedField(hud, "businessesButton", bizBtn);
            UIFactory.SetSerializedField(hud, "financeButton", finBtn);
            UIFactory.SetSerializedField(hud, "marketingButton", mktBtn);
            UIFactory.SetSerializedField(hud, "dailyRewardButton", dailyBtn);
            UIFactory.SetSerializedField(hud, "shopButton", shopBtn);
            UIFactory.SetSerializedField(hud, "settingsButton", setBtn);
            UIFactory.SetSerializedField(hud, "businessListPanel", businessPanel);
            UIFactory.SetSerializedField(hud, "financePanel", financePanel);
            UIFactory.SetSerializedField(hud, "marketingPanel", marketingPanel);
            UIFactory.SetSerializedField(hud, "dailyRewardPanel", dailyPanel);
            UIFactory.SetSerializedField(hud, "settingsPanel", settingsPanel);
            UIFactory.SetSerializedField(hud, "shopPanel", shopPanel);
            UIFactory.SetSerializedField(hud, "leaderboardPanel", leaderboardPanel);
            UIFactory.SetSerializedField(hud, "profitReportPanel", profitPanel);

            new GameObject("GameBootstrap").AddComponent<GameBootstrap>();
            EditorSceneManager.SaveScene(scene, GameScenePath);
        }

        /// <summary>Placeholder 3D building and employee prefabs for the empire view.</summary>
        private static void BuildWorldPrefabs()
        {
            CreateFolder("Assets/Prefabs/World");

            var building = GameObject.CreatePrimitive(PrimitiveType.Cube);
            building.name = "BuildingSlot";
            building.transform.localScale = new Vector3(1.2f, 1.5f, 1.2f);
            var buildingRenderer = building.GetComponent<Renderer>();
            if (buildingRenderer != null)
                buildingRenderer.sharedMaterial = new Material(Shader.Find("Standard")) { color = UITheme.AccentPrimary };

            var buildingLabel = new GameObject("Label");
            buildingLabel.transform.SetParent(building.transform, false);
            buildingLabel.transform.localPosition = new Vector3(0, 1.2f, 0);
            var tm = buildingLabel.AddComponent<TextMesh>();
            tm.text = "Business";
            tm.fontSize = 48;
            tm.characterSize = 0.05f;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.color = Color.white;

            SavePrefab(building, "Assets/Prefabs/World/BuildingSlot");

            var employee = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            employee.name = "EmployeeBadge";
            employee.transform.localScale = new Vector3(0.4f, 0.6f, 0.4f);
            var empRenderer = employee.GetComponent<Renderer>();
            if (empRenderer != null)
                empRenderer.sharedMaterial = new Material(Shader.Find("Standard")) { color = UITheme.AccentSecondary };

            SavePrefab(employee, "Assets/Prefabs/World/EmployeeBadge");
        }

        #endregion

        #region Panel Builders

        private static BusinessListPanel BuildBusinessListPanel(Transform parent, BusinessCardUI cardPrefab,
            out BusinessDetailPanel detailPanel)
        {
            var shell = UIFactory.CreatePopupShell(parent, "Businesses", out var animator, out var close);
            var scroll = UIFactory.CreateScrollView(shell.transform.Find("Panel"), "Scroll", out var content);

            detailPanel = BuildBusinessDetailPanel(parent);

            var panel = shell.AddComponent<BusinessListPanel>();
            UIFactory.SetSerializedField(panel, "animator", animator);
            UIFactory.SetSerializedField(panel, "listContainer", content);
            UIFactory.SetSerializedField(panel, "cardPrefab", cardPrefab);
            UIFactory.SetSerializedField(panel, "detailPanel", detailPanel);
            UIFactory.SetSerializedField(panel, "closeButton", close);
            return panel;
        }

        private static BusinessDetailPanel BuildBusinessDetailPanel(Transform parent)
        {
            var shell = UIFactory.CreatePopupShell(parent, "Business Detail", out var animator, out var close);
            var panelRoot = shell.transform.Find("Panel");

            var title = UIFactory.CreateText(panelRoot, "Title", "Business", 40, UITheme.TextPrimary);
            var desc = UIFactory.CreateText(panelRoot, "Description", "Description", 26, UITheme.TextSecondary);
            var level = UIFactory.CreateText(panelRoot, "Level", "Level 1", 28, UITheme.TextPrimary);
            var income = UIFactory.CreateText(panelRoot, "Income", "Income", 26, UITheme.AccentSuccess);
            var salary = UIFactory.CreateText(panelRoot, "Salary", "Salary", 26, UITheme.AccentDanger);
            var profit = UIFactory.CreateText(panelRoot, "Profit", "Profit", 30, UITheme.AccentPrimary);
            var lifetime = UIFactory.CreateText(panelRoot, "Lifetime", "Lifetime", 24, UITheme.TextSecondary);

            var purchaseBtn = UIFactory.CreateButton(panelRoot, "Purchase", "Buy", UITheme.AccentSuccess);
            var upgradeBtn = UIFactory.CreateButton(panelRoot, "Upgrade", "Upgrade", UITheme.AccentPrimary);
            var hireBtn = UIFactory.CreateButton(panelRoot, "Hire", "Hire", UITheme.AccentWarning);

            var detail = shell.AddComponent<BusinessDetailPanel>();
            UIFactory.SetSerializedField(detail, "animator", animator);
            UIFactory.SetSerializedField(detail, "titleText", title);
            UIFactory.SetSerializedField(detail, "descriptionText", desc);
            UIFactory.SetSerializedField(detail, "levelText", level);
            UIFactory.SetSerializedField(detail, "incomeText", income);
            UIFactory.SetSerializedField(detail, "salaryText", salary);
            UIFactory.SetSerializedField(detail, "profitText", profit);
            UIFactory.SetSerializedField(detail, "lifetimeProfitText", lifetime);
            UIFactory.SetSerializedField(detail, "purchaseButton", purchaseBtn);
            UIFactory.SetSerializedField(detail, "upgradeButton", upgradeBtn);
            UIFactory.SetSerializedField(detail, "hireButton", hireBtn);
            UIFactory.SetSerializedField(detail, "closeButton", close);
            return detail;
        }

        private static FinancePanel BuildFinancePanel(Transform parent, FinanceOptionUI prefab)
        {
            var shell = UIFactory.CreatePopupShell(parent, "Finance", out var animator, out var close);
            var panelRoot = shell.transform.Find("Panel");

            UIFactory.CreateText(panelRoot, "LoansLabel", "Bank Loans", 32, UITheme.TextSecondary);
            var loanContainer = new GameObject("LoanContainer").transform;
            loanContainer.SetParent(panelRoot, false);

            UIFactory.CreateText(panelRoot, "InvLabel", "Investments", 32, UITheme.TextSecondary);
            var invContainer = new GameObject("InvContainer").transform;
            invContainer.SetParent(panelRoot, false);

            var panel = shell.AddComponent<FinancePanel>();
            UIFactory.SetSerializedField(panel, "animator", animator);
            UIFactory.SetSerializedField(panel, "loanContainer", loanContainer);
            UIFactory.SetSerializedField(panel, "investmentContainer", invContainer);
            UIFactory.SetSerializedField(panel, "optionPrefab", prefab);
            UIFactory.SetSerializedField(panel, "closeButton", close);
            return panel;
        }

        private static MarketingPanel BuildMarketingPanel(Transform parent, FinanceOptionUI prefab)
        {
            var shell = UIFactory.CreatePopupShell(parent, "Marketing & R&D", out var animator, out var close);
            var panelRoot = shell.transform.Find("Panel");

            var dropdownGo = new GameObject("BusinessDropdown");
            dropdownGo.transform.SetParent(panelRoot, false);
            var dropdown = dropdownGo.AddComponent<TMP_Dropdown>();

            var campaignContainer = new GameObject("CampaignContainer").transform;
            campaignContainer.SetParent(panelRoot, false);
            var researchContainer = new GameObject("ResearchContainer").transform;
            researchContainer.SetParent(panelRoot, false);

            var panel = shell.AddComponent<MarketingPanel>();
            UIFactory.SetSerializedField(panel, "animator", animator);
            UIFactory.SetSerializedField(panel, "campaignContainer", campaignContainer);
            UIFactory.SetSerializedField(panel, "researchContainer", researchContainer);
            UIFactory.SetSerializedField(panel, "optionPrefab", prefab);
            UIFactory.SetSerializedField(panel, "businessDropdown", dropdown);
            UIFactory.SetSerializedField(panel, "closeButton", close);
            return panel;
        }

        private static DailyRewardPanel BuildDailyRewardPanel(Transform parent)
        {
            var shell = UIFactory.CreatePopupShell(parent, "Daily Reward", out var animator, out var close);
            var panelRoot = shell.transform.Find("Panel");

            var rewardText = UIFactory.CreateText(panelRoot, "Reward", "Day 1 reward", 34, UITheme.AccentWarning);
            var streakText = UIFactory.CreateText(panelRoot, "Streak", "Streak: 0", 28, UITheme.TextSecondary);
            var claimBtn = UIFactory.CreateButton(panelRoot, "Claim", "Claim", UITheme.AccentSuccess);
            var adBtn = UIFactory.CreateButton(panelRoot, "ClaimAd", "Claim x2 (Watch Ad)", UITheme.AccentPremium);

            var panel = shell.AddComponent<DailyRewardPanel>();
            UIFactory.SetSerializedField(panel, "animator", animator);
            UIFactory.SetSerializedField(panel, "rewardText", rewardText);
            UIFactory.SetSerializedField(panel, "streakText", streakText);
            UIFactory.SetSerializedField(panel, "claimButton", claimBtn);
            UIFactory.SetSerializedField(panel, "claimWithAdButton", adBtn);
            UIFactory.SetSerializedField(panel, "closeButton", close);
            return panel;
        }

        private static SettingsPanel BuildSettingsPanel(Transform parent, out SettingsPanel panel,
            LeaderboardPanel leaderboard = null)
        {
            var shell = UIFactory.CreatePopupShell(parent, "Settings", out var animator, out var close);
            var panelRoot = shell.transform.Find("Panel");

            var masterSlider = UIFactory.CreateSlider(panelRoot, "MasterVolume", "Master Volume");
            var sfxSlider = UIFactory.CreateSlider(panelRoot, "SfxVolume", "SFX Volume");
            var haptic = UIFactory.CreateToggle(panelRoot, "Haptic", "Haptic Feedback");
            var notif = UIFactory.CreateToggle(panelRoot, "Notifications", "Notifications");
            var cloudText = UIFactory.CreateText(panelRoot, "CloudStatus", "Cloud save: coming soon", 24,
                UITheme.TextMuted);
            var removeAds = UIFactory.CreateButton(panelRoot, "RemoveAds", "Remove Ads", UITheme.AccentPrimary);
            var restore = UIFactory.CreateButton(panelRoot, "Restore", "Restore Purchases", UITheme.BackgroundElevated);
            var leaderboardBtn = UIFactory.CreateButton(panelRoot, "Leaderboard", "Leaderboard", UITheme.AccentWarning);
            var reset = UIFactory.CreateButton(panelRoot, "Reset", "Reset Progress", UITheme.AccentDanger);

            panel = shell.AddComponent<SettingsPanel>();
            UIFactory.SetSerializedField(panel, "animator", animator);
            UIFactory.SetSerializedField(panel, "masterVolumeSlider", masterSlider);
            UIFactory.SetSerializedField(panel, "sfxVolumeSlider", sfxSlider);
            UIFactory.SetSerializedField(panel, "hapticToggle", haptic);
            UIFactory.SetSerializedField(panel, "notificationsToggle", notif);
            UIFactory.SetSerializedField(panel, "cloudSaveStatusText", cloudText);
            UIFactory.SetSerializedField(panel, "removeAdsButton", removeAds);
            UIFactory.SetSerializedField(panel, "restorePurchasesButton", restore);
            UIFactory.SetSerializedField(panel, "leaderboardButton", leaderboardBtn);
            UIFactory.SetSerializedField(panel, "resetButton", reset);
            UIFactory.SetSerializedField(panel, "closeButton", close);
            if (leaderboard != null)
                UIFactory.SetSerializedField(panel, "leaderboardPanel", leaderboard);
            return panel;
        }

        private static ShopPanel BuildShopPanel(Transform parent, IapProductCatalog catalog, out ShopPanel panel)
        {
            var shell = UIFactory.CreatePopupShell(parent, "Shop", out var animator, out var close);
            var scroll = UIFactory.CreateScrollView(shell.transform.Find("Panel"), "Scroll", out var content);
            var status = UIFactory.CreateText(shell.transform.Find("Panel"), "Status", "Loading shop…", 24,
                UITheme.TextMuted);

            var shopPrefab = AssetDatabase.LoadAssetAtPath<ShopProductUI>($"{PrefabPath}/ShopProductUI.prefab");

            panel = shell.AddComponent<ShopPanel>();
            UIFactory.SetSerializedField(panel, "animator", animator);
            UIFactory.SetSerializedField(panel, "productContainer", content);
            UIFactory.SetSerializedField(panel, "productPrefab", shopPrefab);
            UIFactory.SetSerializedField(panel, "statusText", status);
            UIFactory.SetSerializedField(panel, "closeButton", close);
            UIFactory.SetSerializedField(panel, "catalog", catalog);
            return panel;
        }

        private static LeaderboardPanel BuildLeaderboardPanel(Transform parent, LeaderboardEntryUI prefab)
        {
            var shell = UIFactory.CreatePopupShell(parent, "Leaderboard", out var animator, out var close);
            var panelRoot = shell.transform.Find("Panel");
            var scroll = UIFactory.CreateScrollView(panelRoot, "Scroll", out var content);

            var status = UIFactory.CreateText(panelRoot, "Status", "Sign in to Google Play Games", 24,
                UITheme.TextMuted, TextAlignmentOptions.Top);
            status.rectTransform.anchorMin = new Vector2(0, 1);
            status.rectTransform.anchorMax = new Vector2(1, 1);
            status.rectTransform.sizeDelta = new Vector2(0, 40);
            status.rectTransform.anchoredPosition = new Vector2(0, -90);

            var btnRow = new GameObject("Buttons");
            btnRow.transform.SetParent(panelRoot, false);
            var rowRt = btnRow.AddComponent<RectTransform>();
            rowRt.anchorMin = new Vector2(0, 0);
            rowRt.anchorMax = new Vector2(1, 0);
            rowRt.sizeDelta = new Vector2(0, 100);

            var hlg = btnRow.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 12;
            hlg.padding = new RectOffset(16, 16, 8, 8);
            hlg.childControlWidth = true;
            hlg.childForceExpandWidth = true;

            var signInBtn = UIFactory.CreateButton(btnRow.transform, "SignIn", "Sign In", UITheme.AccentPrimary, 80);
            var refreshBtn = UIFactory.CreateButton(btnRow.transform, "Refresh", "Refresh", UITheme.BackgroundElevated, 80);
            var submitBtn = UIFactory.CreateButton(btnRow.transform, "Submit", "Submit", UITheme.AccentSuccess, 80);
            var nativeBtn = UIFactory.CreateButton(btnRow.transform, "Native", "Google UI", UITheme.AccentWarning, 80);

            var panel = shell.AddComponent<LeaderboardPanel>();
            UIFactory.SetSerializedField(panel, "animator", animator);
            UIFactory.SetSerializedField(panel, "listContainer", content);
            UIFactory.SetSerializedField(panel, "entryPrefab", prefab);
            UIFactory.SetSerializedField(panel, "statusText", status);
            UIFactory.SetSerializedField(panel, "closeButton", close);
            UIFactory.SetSerializedField(panel, "signInButton", signInBtn);
            UIFactory.SetSerializedField(panel, "refreshButton", refreshBtn);
            UIFactory.SetSerializedField(panel, "submitScoreButton", submitBtn);
            UIFactory.SetSerializedField(panel, "nativeUiButton", nativeBtn);
            return panel;
        }

        private static ProfitReportPanel BuildProfitReportPanel(Transform parent)
        {
            var shell = UIFactory.CreatePopupShell(parent, "Profit Report", out var animator, out var close);
            var summary = UIFactory.CreateText(shell.transform.Find("Panel"), "Summary", "Report", 28,
                UITheme.TextPrimary, TextAlignmentOptions.TopLeft);

            var panel = shell.AddComponent<ProfitReportPanel>();
            UIFactory.SetSerializedField(panel, "animator", animator);
            UIFactory.SetSerializedField(panel, "summaryText", summary);
            UIFactory.SetSerializedField(panel, "closeButton", close);
            return panel;
        }

        private static OfflineIncomePopup BuildOfflineIncomePopup(Transform parent)
        {
            var shell = UIFactory.CreatePopupShell(parent, "Welcome Back", out var animator, out _);
            var amount = UIFactory.CreateText(shell.transform.Find("Panel"), "Amount", "You earned $0!", 34,
                UITheme.AccentSuccess);
            var collect = UIFactory.CreateButton(shell.transform.Find("Panel"), "Collect", "Collect",
                UITheme.AccentPrimary);
            var doubleAd = UIFactory.CreateButton(shell.transform.Find("Panel"), "Double", "Collect x2 (Ad)",
                UITheme.AccentPremium);

            var popup = shell.AddComponent<OfflineIncomePopup>();
            UIFactory.SetSerializedField(popup, "animator", animator);
            UIFactory.SetSerializedField(popup, "amountText", amount);
            UIFactory.SetSerializedField(popup, "collectButton", collect);
            UIFactory.SetSerializedField(popup, "doubleWithAdButton", doubleAd);
            return popup;
        }

        private static EventNotificationUI BuildEventNotification(Transform parent)
        {
            var toast = UIFactory.CreatePanel(parent, "EventToast", UITheme.BackgroundElevated, false);
            var rt = toast.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.05f, 0.85f);
            rt.anchorMax = new Vector2(0.95f, 0.95f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            toast.SetActive(false);

            var cg = toast.AddComponent<CanvasGroup>();
            var animator = toast.AddComponent<UIAnimator>();
            var so = new SerializedObject(animator);
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.FindProperty("panelTransform").objectReferenceValue = rt;
            so.ApplyModifiedPropertiesWithoutUndo();

            var title = UIFactory.CreateText(toast.transform, "Title", "Event", 30, UITheme.AccentWarning);
            var msg = UIFactory.CreateText(toast.transform, "Message", "Message", 24, UITheme.TextSecondary);

            var ui = toast.AddComponent<EventNotificationUI>();
            UIFactory.SetSerializedField(ui, "animator", animator);
            UIFactory.SetSerializedField(ui, "titleText", title);
            UIFactory.SetSerializedField(ui, "messageText", msg);
            return ui;
        }

        #endregion

        private static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<EventSystem>() != null) return;
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        private static void ConfigureBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(BootstrapPath, true),
                new EditorBuildSettingsScene(MainMenuPath, true),
                new EditorBuildSettingsScene(GameScenePath, true)
            };
        }
    }
}
#endif
