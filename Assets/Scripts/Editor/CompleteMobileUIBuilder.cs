#if UNITY_EDITOR
using BusinessEmpire.Core;
using BusinessEmpire.UI;
using BusinessEmpire.UI.Screens;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BusinessEmpire.Editor
{
    /// <summary>
    /// Builds the complete 20-screen mobile UI prototype.
    /// Invoked by Business Empire → Build Complete UI
    /// </summary>
    public static class CompleteMobileUIBuilder
    {
        private const string PrefabPath = "Assets/Prefabs/UI";
        private const string PrototypeScene = "Assets/Scenes/UIMobilePrototype.unity";

        private static UIListRowView _rowPrefab;
        private static UICardView _cardPrefab;

        public static void Build()
        {
            EnsureFolder("Assets/Scenes");
            EnsureFolder(PrefabPath);

            _rowPrefab = BuildListRowPrefab();
            _cardPrefab = BuildCardPrefab();

            BuildPrototypeScene();

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/Bootstrap.unity", true),
                new EditorBuildSettingsScene(PrototypeScene, true),
                new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/GameScene.unity", true)
            };

            AssetDatabase.SaveAssets();
            Debug.Log("[Business Empire] Mobile UI prototype built → Open UIMobilePrototype scene and press Play.");
        }

        #region Prefabs

        private static UIListRowView BuildListRowPrefab()
        {
            var root = UIFactory.CreatePanel(null, "UIListRow", UITheme.BackgroundCard, false);
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(980, 120);
            var le = root.AddComponent<LayoutElement>();
            le.minHeight = 120;
            le.preferredHeight = 120;

            var icon = UIFactory.CreatePanel(root.transform, "Icon", UITheme.AccentPrimary, false);
            var iconRt = icon.GetComponent<RectTransform>();
            iconRt.anchorMin = new Vector2(0, 0.5f);
            iconRt.anchorMax = new Vector2(0, 0.5f);
            iconRt.sizeDelta = new Vector2(72, 72);
            iconRt.anchoredPosition = new Vector2(52, 0);

            var title = UIFactory.CreateText(root.transform, "Title", "Title", 32, UITheme.TextPrimary, TextAlignmentOptions.Left);
            title.rectTransform.anchorMin = new Vector2(0, 0.52f);
            title.rectTransform.anchorMax = new Vector2(0.72f, 1);
            title.rectTransform.offsetMin = new Vector2(100, 0);

            var sub = UIFactory.CreateText(root.transform, "Subtitle", "Subtitle", 24, UITheme.TextSecondary, TextAlignmentOptions.Left);
            sub.rectTransform.anchorMin = new Vector2(0, 0);
            sub.rectTransform.anchorMax = new Vector2(0.72f, 0.52f);
            sub.rectTransform.offsetMin = new Vector2(100, 0);

            var val = UIFactory.CreateText(root.transform, "Value", "$0", 28, UITheme.TextAccent, TextAlignmentOptions.Right);
            val.rectTransform.anchorMin = new Vector2(0.68f, 0);
            val.rectTransform.anchorMax = new Vector2(1, 1);
            val.rectTransform.offsetMax = new Vector2(-16, 0);

            var btn = root.AddComponent<Button>();
            btn.targetGraphic = root.GetComponent<Image>();

            var view = root.AddComponent<UIListRowView>();
            UIFactory.SetSerializedField(view, "titleText", title);
            UIFactory.SetSerializedField(view, "subtitleText", sub);
            UIFactory.SetSerializedField(view, "valueText", val);
            UIFactory.SetSerializedField(view, "iconPlaceholder", icon.GetComponent<Image>());
            UIFactory.SetSerializedField(view, "actionButton", btn);

            return SavePrefab(root, "UIListRow").GetComponent<UIListRowView>();
        }

        private static UICardView BuildCardPrefab()
        {
            var root = UIFactory.CreatePanel(null, "UICard", UITheme.BackgroundElevated, false);
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(480, 160);

            var accent = UIFactory.CreatePanel(root.transform, "Accent", UITheme.AccentPrimary, false);
            accent.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            accent.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            accent.GetComponent<RectTransform>().sizeDelta = new Vector2(6, 0);

            var icon = UIFactory.CreatePanel(root.transform, "Icon", UITheme.AccentSecondary, false);
            icon.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
            icon.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(48, 48);
            icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(-32, 0);

            var title = UIFactory.CreateText(root.transform, "Title", "Stat", 26, UITheme.TextSecondary, TextAlignmentOptions.TopLeft);
            title.rectTransform.offsetMin = new Vector2(20, 0);
            title.rectTransform.anchorMin = new Vector2(0, 0.55f);
            title.rectTransform.anchorMax = new Vector2(0.85f, 1);

            var val = UIFactory.CreateText(root.transform, "Value", "0", 40, UITheme.TextPrimary, TextAlignmentOptions.BottomLeft);
            val.rectTransform.offsetMin = new Vector2(20, 12);
            val.rectTransform.anchorMin = new Vector2(0, 0);
            val.rectTransform.anchorMax = new Vector2(0.85f, 0.55f);

            var sub = UIFactory.CreateText(root.transform, "Subtitle", "", 22, UITheme.TextMuted, TextAlignmentOptions.BottomLeft);
            sub.rectTransform.offsetMin = new Vector2(20, 0);
            sub.rectTransform.anchorMax = new Vector2(0.85f, 0.3f);

            var view = root.AddComponent<UICardView>();
            UIFactory.SetSerializedField(view, "titleText", title);
            UIFactory.SetSerializedField(view, "valueText", val);
            UIFactory.SetSerializedField(view, "subtitleText", sub);
            UIFactory.SetSerializedField(view, "accentBar", accent.GetComponent<Image>());
            UIFactory.SetSerializedField(view, "iconPlaceholder", icon.GetComponent<Image>());

            return SavePrefab(root, "UICard").GetComponent<UICardView>();
        }

        private static GameObject SavePrefab(GameObject temp, string name)
        {
            string path = $"{PrefabPath}/{name}.prefab";
            var prefab = PrefabUtility.SaveAsPrefabAsset(temp, path);
            Object.DestroyImmediate(temp);
            return prefab;
        }

        #endregion

        #region Scene

        private static void BuildPrototypeScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EnsureEventSystem();

            var canvas = UIFactory.CreateCanvas("MobileUI");
            var safeRoot = UIFactory.CreatePanel(canvas.transform, "SafeArea", new Color(0, 0, 0, 0));
            safeRoot.AddComponent<UISafeAreaFitter>();

            // Background gradient strip
            var bgTop = UIFactory.CreatePanel(safeRoot.transform, "BgTop", UITheme.BackgroundGradientTop);
            bgTop.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
            UIFactory.StretchFull(bgTop.GetComponent<RectTransform>());
            var bgBottom = UIFactory.CreatePanel(safeRoot.transform, "BgBottom", UITheme.BackgroundDark);
            UIFactory.StretchFull(bgBottom.GetComponent<RectTransform>());

            var topBar = BuildTopBar(safeRoot.transform);
            var bottomNav = BuildBottomNav(safeRoot.transform);
            var content = CreateContentArea(safeRoot.transform, topBar, bottomNav);
            var popups = CreatePopupLayer(safeRoot.transform);

            BuildAllScreens(content, popups);

            var navGo = new GameObject("UINavigationManager");
            navGo.transform.SetParent(canvas.transform, false);
            var nav = navGo.AddComponent<UINavigationManager>();
            UIFactory.SetSerializedField(nav, "topBar", topBar.GetComponent<UITopBar>());
            UIFactory.SetSerializedField(nav, "bottomNav", bottomNav.GetComponent<UIBottomNav>());
            UIFactory.SetSerializedField(nav, "screenContainer", content);
            UIFactory.SetSerializedField(nav, "popupContainer", popups);
            UIFactory.SetSerializedField(nav, "startWithSplash", true);

            new GameObject("GameBootstrap").AddComponent<GameBootstrap>();
            var proto = new GameObject("UIPrototypeMode");
            proto.AddComponent<UIPrototypeBootstrap>();

            EditorSceneManager.SaveScene(scene, PrototypeScene);
        }

        private static RectTransform CreateContentArea(Transform parent, GameObject topBar, GameObject bottomNav)
        {
            var go = new GameObject("ScreenContainer");
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(0, UITheme.BottomNavHeight);
            rt.offsetMax = new Vector2(0, -UITheme.TopBarHeight);
            return rt;
        }

        private static Transform CreatePopupLayer(Transform parent)
        {
            var go = UIFactory.CreatePanel(parent, "PopupContainer", new Color(0, 0, 0, 0));
            UIFactory.StretchFull(go.GetComponent<RectTransform>());
            return go.transform;
        }

        private static GameObject BuildTopBar(Transform parent)
        {
            var bar = UIFactory.CreatePanel(parent, "TopBar", UITheme.BackgroundCard, false);
            var rt = bar.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, UITheme.TopBarHeight);

            var cash = UIFactory.CreateText(bar.transform, "Cash", "$0", 28, UITheme.AccentSuccess, TextAlignmentOptions.Left);
            cash.rectTransform.anchorMin = new Vector2(0, 0.5f);
            cash.rectTransform.anchorMax = new Vector2(0.5f, 1);
            cash.rectTransform.offsetMin = new Vector2(24, 0);

            var gems = UIFactory.CreateText(bar.transform, "Gems", "0", 28, UITheme.AccentPremium, TextAlignmentOptions.Right);
            gems.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            gems.rectTransform.anchorMax = new Vector2(1, 1);
            gems.rectTransform.offsetMax = new Vector2(-80, 0);

            var level = UIFactory.CreateText(bar.transform, "Level", "Lv.1", 24, UITheme.TextSecondary, TextAlignmentOptions.Left);
            level.rectTransform.anchorMin = new Vector2(0, 0);
            level.rectTransform.anchorMax = new Vector2(0.35f, 0.5f);
            level.rectTransform.offsetMin = new Vector2(24, 0);

            var repText = UIFactory.CreateText(bar.transform, "Rep", "Rep 50%", 24, UITheme.TextAccent, TextAlignmentOptions.Right);
            repText.rectTransform.anchorMin = new Vector2(0.35f, 0);
            repText.rectTransform.anchorMax = new Vector2(0.85f, 0.5f);

            var sliderGo = new GameObject("RepSlider");
            sliderGo.transform.SetParent(bar.transform, false);
            var srt = sliderGo.AddComponent<RectTransform>();
            srt.anchorMin = new Vector2(0.1f, 0.06f);
            srt.anchorMax = new Vector2(0.9f, 0.14f);
            var slider = sliderGo.AddComponent<Slider>();
            UIFactory.CreatePanel(sliderGo.transform, "Fill", UITheme.AccentPrimary);

            var settingsBtn = UIFactory.CreateButton(bar.transform, "Settings", "⚙", UITheme.BackgroundElevated, 56);
            settingsBtn.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
            settingsBtn.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
            settingsBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(56, 56);
            settingsBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-36, 0);

            var top = bar.AddComponent<UITopBar>();
            UIFactory.SetSerializedField(top, "cashText", cash);
            UIFactory.SetSerializedField(top, "gemsText", gems);
            UIFactory.SetSerializedField(top, "levelText", level);
            UIFactory.SetSerializedField(top, "reputationText", repText);
            UIFactory.SetSerializedField(top, "reputationSlider", slider);
            UIFactory.SetSerializedField(top, "settingsButton", settingsBtn);
            return bar;
        }

        private static GameObject BuildBottomNav(Transform parent)
        {
            var nav = UIFactory.CreatePanel(parent, "BottomNav", UITheme.BackgroundCard, false);
            var rt = nav.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(0.5f, 0);
            rt.sizeDelta = new Vector2(0, UITheme.BottomNavHeight);

            var grid = nav.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(200, 140);
            grid.spacing = new Vector2(8, 0);
            grid.padding = new RectOffset(16, 16, 12, 12);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 5;

            var tabs = new (UIScreenId id, string label, Color color)[]
            {
                (UIScreenId.Dashboard, "Home", UITheme.AccentPrimary),
                (UIScreenId.BusinessList, "Biz", UITheme.AccentSecondary),
                (UIScreenId.EmployeeHiring, "Staff", UITheme.AccentSuccess),
                (UIScreenId.Reports, "Reports", UITheme.AccentWarning),
                (UIScreenId.Shop, "Shop", UITheme.AccentPremium)
            };

            var navComp = nav.AddComponent<UIBottomNav>();
            var tabArray = new UIBottomNav.NavTab[tabs.Length];

            for (int i = 0; i < tabs.Length; i++)
            {
                var btn = UIFactory.CreateButton(nav.transform, tabs[i].label, tabs[i].label, UITheme.BackgroundElevated, 100);
                var icon = btn.transform.Find(btn.name)?.GetComponent<Image>();
                if (icon == null) icon = btn.GetComponent<Image>();
                var label = btn.GetComponentInChildren<TextMeshProUGUI>();
                tabArray[i] = new UIBottomNav.NavTab
                {
                    screenId = tabs[i].id,
                    button = btn,
                    iconPlaceholder = icon,
                    label = label,
                    activeColor = tabs[i].color
                };
            }

            var so = new SerializedObject(navComp);
            so.FindProperty("tabs").arraySize = tabs.Length;
            for (int i = 0; i < tabs.Length; i++)
            {
                var el = so.FindProperty("tabs").GetArrayElementAtIndex(i);
                el.FindPropertyRelative("screenId").enumValueIndex = (int)tabArray[i].screenId;
                el.FindPropertyRelative("button").objectReferenceValue = tabArray[i].button;
                el.FindPropertyRelative("iconPlaceholder").objectReferenceValue = tabArray[i].iconPlaceholder;
                el.FindPropertyRelative("label").objectReferenceValue = tabArray[i].label;
                el.FindPropertyRelative("activeColor").colorValue = tabArray[i].activeColor;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            return nav;
        }

        private static void BuildAllScreens(Transform content, Transform popups)
        {
            BuildSplash(content);
            BuildMainMenu(content);
            BuildSaveSelect(content);
            BuildDashboard(content);
            BuildBusinessList(content);
            BuildBusinessDetail(content);
            BuildBuyBusiness(content);
            BuildUpgradeBusiness(content);
            BuildEmployeeHiring(content);
            BuildMarketing(content);
            BuildBankLoan(content);
            BuildInvestment(content);
            BuildReports(content);
            BuildDailyRewards(content);
            BuildAchievements(content);
            BuildSettings(content);
            BuildShop(content);
            BuildGameOver(content);
            BuildLevelUpPopup(popups);
            BuildOfflinePopup(popups);
        }

        private static GameObject CreateScreenPanel(Transform parent, string name)
        {
            var panel = UIFactory.CreatePanel(parent, name, new Color(0, 0, 0, 0));
            UIFactory.StretchFull(panel.GetComponent<RectTransform>());
            panel.SetActive(false);
            return panel;
        }

        private static ScrollRect AddScrollContent(Transform parent, out Transform content)
        {
            var scroll = UIFactory.CreateScrollView(parent, "Scroll", out content);
            return scroll;
        }

        private static void BuildSplash(Transform parent)
        {
            var p = CreateScreenPanel(parent, "Splash");
            p.AddComponent<SplashScreen>();
            var logo = UIFactory.CreatePanel(p.transform, "Logo", UITheme.AccentPrimary, false);
            logo.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
            logo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 200);
            var title = UIFactory.CreateText(p.transform, "Title", GameConstants.GameTitle, 56, UITheme.TextPrimary);
            title.rectTransform.anchoredPosition = new Vector2(0, 0);
            var tag = UIFactory.CreateText(p.transform, "Tagline", "CEO Simulator", 36, UITheme.TextAccent);
            tag.rectTransform.anchoredPosition = new Vector2(0, -80);
            var tap = UIFactory.CreateButton(p.transform, "Tap", "Tap to continue", UITheme.AccentSecondary, 80);
            tap.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -400);

            WireScreen<SplashScreen>(p, s =>
            {
                UIFactory.SetSerializedField(s, "titleText", title);
                UIFactory.SetSerializedField(s, "taglineText", tag);
                UIFactory.SetSerializedField(s, "logoPlaceholder", logo.GetComponent<Image>());
                UIFactory.SetSerializedField(s, "tapToContinue", tap);
            });
        }

        private static void BuildMainMenu(Transform parent)
        {
            var p = CreateScreenPanel(parent, "MainMenu");
            var title = UIFactory.CreateText(p.transform, "Title", GameConstants.GameTitle, 64, UITheme.TextPrimary);
            title.rectTransform.anchorMin = new Vector2(0, 0.7f);
            title.rectTransform.anchorMax = new Vector2(1, 0.8f);
            var hero = UIFactory.CreatePanel(p.transform, "Hero", UITheme.AccentSecondary, false);
            hero.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 240);
            hero.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 120);
            var play = UIFactory.CreateButton(p.transform, "Play", "▶  PLAY", UITheme.AccentPrimary);
            play.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -120);
            var shop = UIFactory.CreateButton(p.transform, "Shop", "💎 SHOP", UITheme.AccentPremium);
            shop.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -240);
            var settings = UIFactory.CreateButton(p.transform, "Settings", "⚙ SETTINGS", UITheme.BackgroundElevated);
            settings.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -360);

            WireScreen<MainMenuScreen>(p, s =>
            {
                UIFactory.SetSerializedField(s, "titleText", title);
                UIFactory.SetSerializedField(s, "playButton", play);
                UIFactory.SetSerializedField(s, "shopButton", shop);
                UIFactory.SetSerializedField(s, "settingsButton", settings);
                UIFactory.SetSerializedField(s, "heroPlaceholder", hero.GetComponent<Image>());
            });
        }

        private static void BuildSaveSelect(Transform parent)
        {
            var p = CreateScreenPanel(parent, "SaveSelect");
            UIFactory.CreateText(p.transform, "Header", "Start Your Empire", 48, UITheme.TextPrimary).rectTransform.anchoredPosition = new Vector2(0, 500);
            var contInfo = UIFactory.CreateText(p.transform, "Info", "Continue save info", 28, UITheme.TextSecondary);
            contInfo.rectTransform.anchoredPosition = new Vector2(0, 200);
            var newBtn = UIFactory.CreateButton(p.transform, "New", "New Game", UITheme.AccentSuccess);
            newBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            var contBtn = UIFactory.CreateButton(p.transform, "Continue", "Continue", UITheme.AccentPrimary);
            contBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -120);
            var back = UIFactory.CreateButton(p.transform, "Back", "Back", UITheme.BackgroundElevated, 72);
            back.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -280);

            WireScreen<SaveSelectScreen>(p, s =>
            {
                UIFactory.SetSerializedField(s, "newGameButton", newBtn);
                UIFactory.SetSerializedField(s, "continueButton", contBtn);
                UIFactory.SetSerializedField(s, "backButton", back);
                UIFactory.SetSerializedField(s, "continueInfoText", contInfo);
            });
        }

        private static void BuildDashboard(Transform parent)
        {
            var p = CreateScreenPanel(parent, "Dashboard");
            var welcome = UIFactory.CreateText(p.transform, "Welcome", "Welcome", 40, UITheme.TextPrimary, TextAlignmentOptions.TopLeft);
            welcome.rectTransform.anchorMin = new Vector2(0, 1);
            welcome.rectTransform.anchorMax = new Vector2(1, 1);
            welcome.rectTransform.sizeDelta = new Vector2(0, 80);
            welcome.rectTransform.anchoredPosition = new Vector2(0, -20);
            welcome.rectTransform.offsetMin = new Vector2(32, 0);

            var grid = new GameObject("StatGrid");
            grid.transform.SetParent(p.transform, false);
            var grt = grid.AddComponent<RectTransform>();
            grt.anchorMin = new Vector2(0, 0.55f);
            grt.anchorMax = new Vector2(1, 0.88f);
            grt.offsetMin = new Vector2(24, 0);
            grt.offsetMax = new Vector2(-24, 0);
            var glg = grid.AddComponent<GridLayoutGroup>();
            glg.cellSize = new Vector2(480, 160);
            glg.spacing = new Vector2(24, 24);
            glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            glg.constraintCount = 2;

            var cards = new UICardView[4];
            for (int i = 0; i < 4; i++)
                cards[i] = (Object.Instantiate(_cardPrefab, grid.transform) as UICardView)!;

            var actions = new GameObject("Actions");
            actions.transform.SetParent(p.transform, false);
            var art = actions.AddComponent<RectTransform>();
            art.anchorMin = new Vector2(0, 0);
            art.anchorMax = new Vector2(1, 0.52f);
            art.offsetMin = new Vector2(32, 32);
            art.offsetMax = new Vector2(-32, 0);
            var vlg = actions.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 12;
            vlg.childControlHeight = true;
            vlg.childForceExpandHeight = false;

            var daily = UIFactory.CreateButton(actions.transform, "Daily", "Daily Rewards", UITheme.AccentWarning);
            var ach = UIFactory.CreateButton(actions.transform, "Ach", "Achievements", UITheme.AccentPremium);
            var mkt = UIFactory.CreateButton(actions.transform, "Mkt", "Marketing", UITheme.AccentSecondary);
            var loan = UIFactory.CreateButton(actions.transform, "Loan", "Bank Loans", UITheme.BackgroundElevated);
            var inv = UIFactory.CreateButton(actions.transform, "Inv", "Investments", UITheme.AccentSuccess);
            var off = UIFactory.CreateButton(actions.transform, "Off", "Preview Offline Earnings", UITheme.BackgroundElevated, 72);

            WireScreen<DashboardScreen>(p, s =>
            {
                UIFactory.SetSerializedField(s, "welcomeText", welcome);
                UIFactory.SetSerializedField(s, "statCards", cards);
                UIFactory.SetSerializedField(s, "dailyRewardButton", daily);
                UIFactory.SetSerializedField(s, "achievementsButton", ach);
                UIFactory.SetSerializedField(s, "marketingButton", mkt);
                UIFactory.SetSerializedField(s, "loansButton", loan);
                UIFactory.SetSerializedField(s, "investmentsButton", inv);
                UIFactory.SetSerializedField(s, "offlinePreviewButton", off);
            });
        }

        private static void BuildScrollListScreen<T>(Transform parent, string name, out Transform listContent, out Button backBtn)
            where T : UIScreenBase
        {
            var p = CreateScreenPanel(parent, name);
            var header = UIFactory.CreateText(p.transform, "Header", name, 44, UITheme.TextPrimary, TextAlignmentOptions.TopLeft);
            header.rectTransform.anchorMin = new Vector2(0, 1);
            header.rectTransform.anchorMax = new Vector2(1, 1);
            header.rectTransform.sizeDelta = new Vector2(0, 70);
            header.rectTransform.anchoredPosition = new Vector2(0, -10);
            header.rectTransform.offsetMin = new Vector2(32, 0);

            var scroll = UIFactory.CreateScrollView(p.transform, "Scroll", out listContent);
            var srt = scroll.GetComponent<RectTransform>();
            srt.anchorMin = new Vector2(0, 0.12f);
            srt.anchorMax = new Vector2(1, 0.92f);
            srt.offsetMin = new Vector2(24, 0);
            srt.offsetMax = new Vector2(-24, 0);

            backBtn = UIFactory.CreateButton(p.transform, "Back", "← Back", UITheme.BackgroundElevated, 72);
            backBtn.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            backBtn.GetComponent<RectTransform>().anchorMax = new Vector2(0.4f, 0);
            backBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 80);
            backBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(140, 50);
            p.AddComponent<T>();
        }

        private static void BuildBusinessList(Transform parent)
        {
            BuildScrollListScreen<BusinessListScreen>(parent, "BusinessList", out var list, out var back);
            WireScreen<BusinessListScreen>(parent.Find("BusinessList").gameObject, s =>
            {
                UIFactory.SetSerializedField(s, "listContainer", list);
                UIFactory.SetSerializedField(s, "rowPrefab", _rowPrefab);
                UIFactory.SetSerializedField(s, "backButton", back);
            });
        }

        private static void BuildBusinessDetail(Transform parent)
        {
            var p = CreateScreenPanel(parent, "BusinessDetail");
            var title = UIFactory.CreateText(p.transform, "Title", "Business", 48, UITheme.TextPrimary, TextAlignmentOptions.TopLeft);
            title.rectTransform.offsetMin = new Vector2(32, 0);
            title.rectTransform.anchorMin = new Vector2(0, 0.85f);
            title.rectTransform.anchorMax = new Vector2(1, 1);
            var icon = UIFactory.CreatePanel(p.transform, "Icon", UITheme.AccentPrimary, false);
            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 120);
            icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300, 200);
            var desc = UIFactory.CreateText(p.transform, "Desc", "Description", 28, UITheme.TextSecondary, TextAlignmentOptions.TopLeft);
            desc.rectTransform.anchorMin = new Vector2(0, 0.55f);
            desc.rectTransform.anchorMax = new Vector2(1, 0.85f);
            desc.rectTransform.offsetMin = new Vector2(32, 0);
            var stats = UIFactory.CreateText(p.transform, "Stats", "Stats", 30, UITheme.TextPrimary, TextAlignmentOptions.TopLeft);
            stats.rectTransform.anchorMin = new Vector2(0, 0.25f);
            stats.rectTransform.anchorMax = new Vector2(1, 0.55f);
            stats.rectTransform.offsetMin = new Vector2(32, 0);
            var up = UIFactory.CreateButton(p.transform, "Up", "Upgrade", UITheme.AccentPrimary);
            up.GetComponent<RectTransform>().anchoredPosition = new Vector2(-200, -500);
            var hire = UIFactory.CreateButton(p.transform, "Hire", "Hire Staff", UITheme.AccentSuccess);
            hire.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -500);
            var mkt = UIFactory.CreateButton(p.transform, "Mkt", "Marketing", UITheme.AccentWarning);
            mkt.GetComponent<RectTransform>().anchoredPosition = new Vector2(200, -500);
            var back = UIFactory.CreateButton(p.transform, "Back", "Back", UITheme.BackgroundElevated, 72);
            back.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -650);

            WireScreen<BusinessDetailScreen>(p, s =>
            {
                UIFactory.SetSerializedField(s, "titleText", title);
                UIFactory.SetSerializedField(s, "descText", desc);
                UIFactory.SetSerializedField(s, "statsText", stats);
                UIFactory.SetSerializedField(s, "iconPlaceholder", icon.GetComponent<Image>());
                UIFactory.SetSerializedField(s, "upgradeButton", up);
                UIFactory.SetSerializedField(s, "hireButton", hire);
                UIFactory.SetSerializedField(s, "marketingButton", mkt);
                UIFactory.SetSerializedField(s, "backButton", back);
            });
        }

        private static void BuildFormScreen(Transform parent, string name, System.Type screenType,
            System.Action<GameObject, TextMeshProUGUI, Button, Button> wire)
        {
            var p = CreateScreenPanel(parent, name);
            var title = UIFactory.CreateText(p.transform, "Title", name, 44, UITheme.TextPrimary);
            title.rectTransform.anchoredPosition = new Vector2(0, 600);
            var body = UIFactory.CreateText(p.transform, "Body", "Details", 30, UITheme.TextSecondary);
            body.rectTransform.sizeDelta = new Vector2(900, 400);
            body.rectTransform.anchoredPosition = new Vector2(0, 150);
            var primary = UIFactory.CreateButton(p.transform, "Primary", "Confirm", UITheme.AccentPrimary);
            primary.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);
            var back = UIFactory.CreateButton(p.transform, "Back", "Cancel", UITheme.BackgroundElevated);
            back.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -330);
            p.AddComponent(screenType);
            wire(p, body, primary, back);
        }

        private static void BuildBuyBusiness(Transform parent)
        {
            BuildFormScreen(parent, "BuyBusiness", typeof(BuyBusinessScreen), (p, body, primary, back) =>
            {
                var title = p.transform.Find("Title").GetComponent<TextMeshProUGUI>();
                var cost = UIFactory.CreateText(p.transform, "Cost", "$0", 52, UITheme.AccentSuccess);
                cost.rectTransform.anchoredPosition = new Vector2(0, 350);
                var income = UIFactory.CreateText(p.transform, "Income", "+$0/day", 32, UITheme.TextAccent);
                income.rectTransform.anchoredPosition = new Vector2(0, 280);
                WireScreen<BuyBusinessScreen>(p, s =>
                {
                    UIFactory.SetSerializedField(s, "titleText", title);
                    UIFactory.SetSerializedField(s, "descText", body);
                    UIFactory.SetSerializedField(s, "costText", cost);
                    UIFactory.SetSerializedField(s, "incomeText", income);
                    UIFactory.SetSerializedField(s, "buyButton", primary);
                    UIFactory.SetSerializedField(s, "cancelButton", back);
                });
            });
        }

        private static void BuildUpgradeBusiness(Transform parent)
        {
            var p = CreateScreenPanel(parent, "UpgradeBusiness");
            p.AddComponent<UpgradeBusinessScreen>();
            var title = UIFactory.CreateText(p.transform, "Title", "Upgrade", 44, UITheme.TextPrimary);
            title.rectTransform.anchoredPosition = new Vector2(0, 600);
            var lvl = UIFactory.CreateText(p.transform, "Level", "Lv 1 → 2", 36, UITheme.TextAccent);
            lvl.rectTransform.anchoredPosition = new Vector2(0, 450);
            var cost = UIFactory.CreateText(p.transform, "Cost", "$0", 40, UITheme.AccentWarning);
            cost.rectTransform.anchoredPosition = new Vector2(0, 350);
            var bonus = UIFactory.CreateText(p.transform, "Bonus", "+15%", 32, UITheme.TextSecondary);
            bonus.rectTransform.anchoredPosition = new Vector2(0, 280);
            var progGo = new GameObject("Progress");
            progGo.transform.SetParent(p.transform, false);
            progGo.AddComponent<RectTransform>().sizeDelta = new Vector2(800, 60);
            progGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150);
            var prog = progGo.AddComponent<UIProgressBarView>();
            var up = UIFactory.CreateButton(p.transform, "Up", "Upgrade Now", UITheme.AccentPrimary);
            up.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100);
            var back = UIFactory.CreateButton(p.transform, "Back", "Back", UITheme.BackgroundElevated, 72);
            back.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250);
            WireScreen<UpgradeBusinessScreen>(p, s =>
            {
                UIFactory.SetSerializedField(s, "titleText", title);
                UIFactory.SetSerializedField(s, "currentLevelText", lvl);
                UIFactory.SetSerializedField(s, "upgradeCostText", cost);
                UIFactory.SetSerializedField(s, "bonusText", bonus);
                UIFactory.SetSerializedField(s, "levelProgress", prog);
                UIFactory.SetSerializedField(s, "upgradeButton", up);
                UIFactory.SetSerializedField(s, "backButton", back);
            });
        }

        private static void BuildEmployeeHiring(Transform parent)
        {
            BuildScrollListScreen<EmployeeHiringScreen>(parent, "EmployeeHiring", out var list, out var back);
            var summary = UIFactory.CreateText(parent.Find("EmployeeHiring"), "Summary", "Total: 0", 28, UITheme.TextSecondary);
            summary.rectTransform.anchorMin = new Vector2(0, 0.08f);
            summary.rectTransform.anchorMax = new Vector2(1, 0.12f);
            WireScreen<EmployeeHiringScreen>(parent.Find("EmployeeHiring").gameObject, s =>
            {
                UIFactory.SetSerializedField(s, "listContainer", list);
                UIFactory.SetSerializedField(s, "rowPrefab", _rowPrefab);
                UIFactory.SetSerializedField(s, "summaryText", summary);
                UIFactory.SetSerializedField(s, "backButton", back);
            });
        }

        private static void BuildMarketing(Transform parent) => BuildSimpleListScreen<MarketingCampaignScreen>(parent, "MarketingCampaign");
        private static void BuildBankLoan(Transform parent) => BuildSimpleListScreen<BankLoanScreen>(parent, "BankLoan");
        private static void BuildInvestment(Transform parent) => BuildSimpleListScreen<InvestmentScreen>(parent, "Investment");

        private static void BuildSimpleListScreen<T>(Transform parent, string name) where T : UIScreenBase
        {
            BuildScrollListScreen<T>(parent, name, out var list, out var back);
            WireScreen<T>(parent.Find(name).gameObject, s =>
            {
                var so = new SerializedObject(s);
                so.FindProperty("listContainer").objectReferenceValue = list;
                so.FindProperty("rowPrefab").objectReferenceValue = _rowPrefab;
                so.FindProperty("backButton").objectReferenceValue = back;
                so.ApplyModifiedPropertiesWithoutUndo();
            });
        }

        private static void BuildReports(Transform parent)
        {
            var p = CreateScreenPanel(parent, "Reports");
            p.AddComponent<ReportsScreen>();
            var summary = UIFactory.CreateText(p.transform, "Summary", "Report", 28, UITheme.TextPrimary, TextAlignmentOptions.TopLeft);
            summary.rectTransform.anchorMin = new Vector2(0, 0.55f);
            summary.rectTransform.anchorMax = new Vector2(1, 0.95f);
            summary.rectTransform.offsetMin = new Vector2(32, 0);
            var scroll = UIFactory.CreateScrollView(p.transform, "Scroll", out var list);
            scroll.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.15f);
            scroll.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.54f);
            var back = UIFactory.CreateButton(p.transform, "Back", "Back", UITheme.BackgroundElevated, 72);
            back.GetComponent<RectTransform>().anchoredPosition = new Vector2(-180, -700);
            var goPreview = UIFactory.CreateButton(p.transform, "GO", "Preview Bankruptcy", UITheme.AccentDanger, 72);
            goPreview.GetComponent<RectTransform>().anchoredPosition = new Vector2(180, -700);
            WireScreen<ReportsScreen>(p, s =>
            {
                UIFactory.SetSerializedField(s, "summaryText", summary);
                UIFactory.SetSerializedField(s, "listContainer", list);
                UIFactory.SetSerializedField(s, "rowPrefab", _rowPrefab);
                UIFactory.SetSerializedField(s, "backButton", back);
                UIFactory.SetSerializedField(s, "gameOverPreviewButton", goPreview);
            });
        }

        private static void BuildDailyRewards(Transform parent)
        {
            BuildScrollListScreen<DailyRewardsScreen>(parent, "DailyRewards", out var list, out var back);
            var streak = UIFactory.CreateText(parent.Find("DailyRewards"), "Streak", "Streak", 28, UITheme.AccentWarning);
            streak.rectTransform.anchorMin = new Vector2(0, 0.92f);
            streak.rectTransform.anchorMax = new Vector2(1, 0.98f);
            var claim = UIFactory.CreateButton(parent.Find("DailyRewards"), "Claim", "Claim Reward", UITheme.AccentSuccess, 80);
            claim.GetComponent<RectTransform>().anchorMin = new Vector2(0.55f, 0);
            claim.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);
            claim.GetComponent<RectTransform>().sizeDelta = new Vector2(-40, 80);
            claim.GetComponent<RectTransform>().anchoredPosition = new Vector2(-20, 50);
            var claimAd = UIFactory.CreateButton(parent.Find("DailyRewards"), "ClaimAd", "Claim + Ad x2", UITheme.AccentPremium, 80);
            claimAd.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            claimAd.GetComponent<RectTransform>().anchorMax = new Vector2(0.45f, 0);
            claimAd.GetComponent<RectTransform>().sizeDelta = new Vector2(-40, 80);
            claimAd.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, 50);
            WireScreen<DailyRewardsScreen>(parent.Find("DailyRewards").gameObject, s =>
            {
                UIFactory.SetSerializedField(s, "gridContainer", list);
                UIFactory.SetSerializedField(s, "dayRowPrefab", _rowPrefab);
                UIFactory.SetSerializedField(s, "streakText", streak);
                UIFactory.SetSerializedField(s, "claimButton", claim);
                UIFactory.SetSerializedField(s, "claimWithAdButton", claimAd);
                UIFactory.SetSerializedField(s, "backButton", back);
            });
        }

        private static void BuildAchievements(Transform parent)
        {
            BuildScrollListScreen<AchievementsScreen>(parent, "Achievements", out var list, out var back);
            var prog = UIFactory.CreateText(parent.Find("Achievements"), "Progress", "0/0", 26, UITheme.TextMuted);
            prog.rectTransform.anchorMin = new Vector2(0, 0.08f);
            prog.rectTransform.anchorMax = new Vector2(1, 0.12f);
            WireScreen<AchievementsScreen>(parent.Find("Achievements").gameObject, s =>
            {
                UIFactory.SetSerializedField(s, "listContainer", list);
                UIFactory.SetSerializedField(s, "rowPrefab", _rowPrefab);
                UIFactory.SetSerializedField(s, "progressText", prog);
                UIFactory.SetSerializedField(s, "backButton", back);
            });
        }

        private static void BuildSettings(Transform parent)
        {
            var p = CreateScreenPanel(parent, "Settings");
            p.AddComponent<SettingsScreen>();
            UIFactory.CreateText(p.transform, "Header", "Settings", 48, UITheme.TextPrimary).rectTransform.anchoredPosition = new Vector2(0, 650);
            var master = UIFactory.CreateSlider(p.transform, "Master", "Master Volume");
            master.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 400);
            var sfx = UIFactory.CreateSlider(p.transform, "SFX", "SFX Volume");
            sfx.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 280);
            var haptic = UIFactory.CreateToggle(p.transform, "Haptic", "Haptic Feedback");
            haptic.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 160);
            var notif = UIFactory.CreateToggle(p.transform, "Notif", "Notifications");
            notif.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 60);
            var reset = UIFactory.CreateButton(p.transform, "Reset", "Reset Progress", UITheme.AccentDanger, 72);
            reset.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -120);
            var goPreview = UIFactory.CreateButton(p.transform, "GO", "Preview Bankruptcy", UITheme.BackgroundElevated, 72);
            goPreview.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -240);
            var monetTest = UIFactory.CreateButton(p.transform, "MonetTest", "Monetization Test Panel", UITheme.AccentWarning, 72);
            monetTest.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -360);
            var back = UIFactory.CreateButton(p.transform, "Back", "Back", UITheme.BackgroundElevated, 72);
            back.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -480);
            var ver = UIFactory.CreateText(p.transform, "Ver", "v1.0", 22, UITheme.TextMuted);
            ver.rectTransform.anchoredPosition = new Vector2(0, -580);
            WireScreen<SettingsScreen>(p, s =>
            {
                UIFactory.SetSerializedField(s, "masterVolumeSlider", master);
                UIFactory.SetSerializedField(s, "sfxVolumeSlider", sfx);
                UIFactory.SetSerializedField(s, "hapticToggle", haptic);
                UIFactory.SetSerializedField(s, "notificationsToggle", notif);
                UIFactory.SetSerializedField(s, "resetButton", reset);
                UIFactory.SetSerializedField(s, "gameOverPreviewButton", goPreview);
                UIFactory.SetSerializedField(s, "monetizationTestButton", monetTest);
                UIFactory.SetSerializedField(s, "backButton", back);
                UIFactory.SetSerializedField(s, "versionText", ver);
            });
        }

        private static void BuildShop(Transform parent)
        {
            BuildScrollListScreen<ShopScreen>(parent, "Shop", out var list, out var back);
            var gems = UIFactory.CreateText(parent.Find("Shop"), "Gems", "Gems: 0", 32, UITheme.AccentPremium);
            gems.rectTransform.anchorMin = new Vector2(0, 0.92f);
            gems.rectTransform.anchorMax = new Vector2(1, 0.98f);
            WireScreen<ShopScreen>(parent.Find("Shop").gameObject, s =>
            {
                UIFactory.SetSerializedField(s, "listContainer", list);
                UIFactory.SetSerializedField(s, "rowPrefab", _rowPrefab);
                UIFactory.SetSerializedField(s, "gemsBalanceText", gems);
                UIFactory.SetSerializedField(s, "backButton", back);
            });
        }

        private static void BuildGameOver(Transform parent)
        {
            var p = CreateScreenPanel(parent, "GameOver");
            p.AddComponent<GameOverScreen>();
            var title = UIFactory.CreateText(p.transform, "Title", "Bankruptcy", 64, UITheme.AccentDanger);
            title.rectTransform.anchoredPosition = new Vector2(0, 300);
            var msg = UIFactory.CreateText(p.transform, "Msg", "Message", 30, UITheme.TextSecondary);
            msg.rectTransform.sizeDelta = new Vector2(900, 300);
            msg.rectTransform.anchoredPosition = new Vector2(0, 50);
            var restart = UIFactory.CreateButton(p.transform, "Restart", "Try Again", UITheme.AccentPrimary);
            restart.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250);
            var menu = UIFactory.CreateButton(p.transform, "Menu", "Main Menu", UITheme.BackgroundElevated);
            menu.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -380);
            WireScreen<GameOverScreen>(p, s =>
            {
                UIFactory.SetSerializedField(s, "titleText", title);
                UIFactory.SetSerializedField(s, "messageText", msg);
                UIFactory.SetSerializedField(s, "restartButton", restart);
                UIFactory.SetSerializedField(s, "mainMenuButton", menu);
            });
        }

        private static void BuildPopup(Transform parent, string name, System.Type type, System.Action<GameObject> wire)
        {
            var overlay = UIFactory.CreatePopupShell(parent, name, out var anim, out var close);
            overlay.SetActive(false);
            overlay.AddComponent(type);
            wire(overlay);
        }

        private static void BuildLevelUpPopup(Transform parent)
        {
            var overlay = UIFactory.CreatePopupShell(parent, "LevelUpPopup", out var anim, out _);
            overlay.SetActive(false);
            var panel = overlay.transform.Find("Panel");
            var title = UIFactory.CreateText(panel, "Title", "Success!", 48, UITheme.AccentSuccess);
            var msg = UIFactory.CreateText(panel, "Msg", "Message", 30, UITheme.TextSecondary);
            msg.rectTransform.sizeDelta = new Vector2(800, 200);
            var ok = UIFactory.CreateButton(panel, "OK", "Awesome!", UITheme.AccentPrimary);
            ok.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);
            var popup = overlay.AddComponent<LevelUpPopup>();
            UIFactory.SetSerializedField(popup, "animator", anim);
            UIFactory.SetSerializedField(popup, "titleText", title);
            UIFactory.SetSerializedField(popup, "messageText", msg);
            UIFactory.SetSerializedField(popup, "okButton", ok);
        }

        private static void BuildOfflinePopup(Transform parent)
        {
            var overlay = UIFactory.CreatePopupShell(parent, "OfflineEarningsPopup", out var anim, out _);
            overlay.SetActive(false);
            var panel = overlay.transform.Find("Panel");
            var title = UIFactory.CreateText(panel, "Title", "Welcome Back!", 44, UITheme.TextPrimary);
            var amount = UIFactory.CreateText(panel, "Amount", "$0", 56, UITheme.AccentSuccess);
            amount.rectTransform.anchoredPosition = new Vector2(0, 50);
            var sub = UIFactory.CreateText(panel, "Sub", "While you were away", 26, UITheme.TextSecondary);
            sub.rectTransform.anchoredPosition = new Vector2(0, -20);
            var collect = UIFactory.CreateButton(panel, "Collect", "Collect", UITheme.AccentPrimary);
            collect.GetComponent<RectTransform>().anchoredPosition = new Vector2(-150, -180);
            var dbl = UIFactory.CreateButton(panel, "Double", "x2 (Ad)", UITheme.AccentPremium);
            dbl.GetComponent<RectTransform>().anchoredPosition = new Vector2(150, -180);
            var popup = overlay.AddComponent<OfflineEarningsPopup>();
            UIFactory.SetSerializedField(popup, "animator", anim);
            UIFactory.SetSerializedField(popup, "amountText", amount);
            UIFactory.SetSerializedField(popup, "subtitleText", sub);
            UIFactory.SetSerializedField(popup, "collectButton", collect);
            UIFactory.SetSerializedField(popup, "doubleButton", dbl);
        }

        private static void WireScreen<T>(GameObject go, UIScreenId id, System.Action<T> wire) where T : UIScreenBase
        {
            var comp = go.GetComponent<T>();
            if (comp == null) comp = go.AddComponent<T>();
            var so = new SerializedObject(comp);
            so.FindProperty("screenId").enumValueIndex = (int)id;
            so.ApplyModifiedPropertiesWithoutUndo();
            wire(comp);
        }

        private static void WireScreen<T>(GameObject go, System.Action<T> wire) where T : UIScreenBase
        {
            var comp = go.GetComponent<T>();
            if (comp == null) comp = go.AddComponent<T>();
            wire(comp);
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<EventSystem>() != null) return;
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parent = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
            var name = System.IO.Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, name);
        }

        #endregion
    }
}
#endif
