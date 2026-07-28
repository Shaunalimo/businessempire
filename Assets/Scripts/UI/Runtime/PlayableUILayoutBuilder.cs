using BusinessEmpire.UI;
using BusinessEmpire.UI.Screens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Runtime
{
    /// <summary>
    /// Builds the full gameplay UI hierarchy. Used by the editor scene populator and runtime fallback builder.
    /// </summary>
    public static class PlayableUILayoutBuilder
    {
        public struct GameplayBuildResult
        {
            public UITopBar TopBar;
            public UIBottomNav BottomNav;
            public Transform ScreenRoot;
            public Transform PopupRoot;
            public UINavigationManager Navigation;
            public Button BuyBusinessButton;
            public Button UpgradeBusinessButton;
            public Button EarnMoneyButton;
        }

        public static GameplayBuildResult BuildGameplayInterface(Transform canvasTransform)
        {
            RuntimeUIFactory.CreatePanel(canvasTransform, "Background", UITheme.BackgroundDark);

            var topBar = BuildTopBar(canvasTransform);
            var bottomNav = BuildBottomNav(canvasTransform);
            var screenRoot = CreateStretchChild(canvasTransform, "Screens");
            screenRoot.offsetMin = new Vector2(0, UITheme.BottomNavHeight);
            screenRoot.offsetMax = new Vector2(0, -UITheme.TopBarHeight);

            var popupRoot = CreateStretchChild(canvasTransform, "Popups");
            var templates = new GameObject("Templates");
            templates.transform.SetParent(canvasTransform, false);
            templates.SetActive(false);
            var rowTemplate = RuntimeUIFactory.CreateListRowPrefab(templates.transform);

            Button buyBtn = null, upgradeBtn = null, earnBtn = null;
            BuildDashboard(screenRoot, out buyBtn, out upgradeBtn, out earnBtn);
            BuildBusinessList(screenRoot, rowTemplate);
            BuildEmployeeScreen(screenRoot, rowTemplate);
            BuildShop(screenRoot, rowTemplate);
            BuildSettings(screenRoot);
            BuildBusinessDetail(screenRoot);
            BuildBuyBusiness(screenRoot);
            BuildUpgradeBusiness(screenRoot);

            var navGo = new GameObject("UINavigationManager");
            var nav = navGo.AddComponent<UINavigationManager>();
            nav.Setup(topBar, bottomNav, screenRoot, popupRoot, splash: false);

            return new GameplayBuildResult
            {
                TopBar = topBar,
                BottomNav = bottomNav,
                ScreenRoot = screenRoot,
                PopupRoot = popupRoot,
                Navigation = nav,
                BuyBusinessButton = buyBtn,
                UpgradeBusinessButton = upgradeBtn,
                EarnMoneyButton = earnBtn
            };
        }

        private static UITopBar BuildTopBar(Transform parent)
        {
            var bar = RuntimeUIFactory.CreatePanel(parent, "TopBar", UITheme.BackgroundCard, false);
            var rt = bar;
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.sizeDelta = new Vector2(0, UITheme.TopBarHeight);

            var cash = RuntimeUIFactory.CreateText(bar, "Cash", "$5,000", 30, UITheme.AccentSuccess, TextAlignmentOptions.Left);
            cash.rectTransform.anchorMin = new Vector2(0, 0.55f);
            cash.rectTransform.anchorMax = new Vector2(0.33f, 1);
            cash.rectTransform.offsetMin = new Vector2(20, 0);

            var gems = RuntimeUIFactory.CreateText(bar, "Gems", "10", 30, UITheme.AccentPremium, TextAlignmentOptions.Left);
            gems.rectTransform.anchorMin = new Vector2(0.33f, 0.55f);
            gems.rectTransform.anchorMax = new Vector2(0.66f, 1);

            var level = RuntimeUIFactory.CreateText(bar, "Level", "Lv.1", 26, UITheme.TextSecondary, TextAlignmentOptions.Left);
            level.rectTransform.anchorMin = new Vector2(0, 0.05f);
            level.rectTransform.anchorMax = new Vector2(0.33f, 0.5f);
            level.rectTransform.offsetMin = new Vector2(20, 0);

            var rep = RuntimeUIFactory.CreateText(bar, "Rep", "50%", 26, UITheme.TextSecondary, TextAlignmentOptions.Left);
            rep.rectTransform.anchorMin = new Vector2(0.33f, 0.05f);
            rep.rectTransform.anchorMax = new Vector2(0.66f, 0.5f);

            var sliderGo = new GameObject("RepSlider");
            sliderGo.transform.SetParent(bar, false);
            var sliderRt = sliderGo.AddComponent<RectTransform>();
            sliderRt.anchorMin = new Vector2(0.05f, 0.02f);
            sliderRt.anchorMax = new Vector2(0.95f, 0.08f);
            sliderRt.offsetMin = Vector2.zero;
            sliderRt.offsetMax = Vector2.zero;
            var slider = sliderGo.AddComponent<Slider>();
            RuntimeUIFactory.CreatePanel(sliderGo.transform, "Fill", UITheme.AccentPrimary);

            var settingsBtn = RuntimeUIFactory.CreateButton(bar, "Settings", "⚙", UITheme.BackgroundElevated, 64);
            settingsBtn.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
            settingsBtn.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
            settingsBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(64, 64);
            settingsBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-40, 0);

            var top = bar.gameObject.AddComponent<UITopBar>();
            top.Setup(cash, gems, level, rep, slider, settingsBtn);
            return top;
        }

        private static UIBottomNav BuildBottomNav(Transform parent)
        {
            var nav = RuntimeUIFactory.CreatePanel(parent, "BottomNav", UITheme.BackgroundCard, false);
            var rt = nav;
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(0.5f, 0);
            rt.sizeDelta = new Vector2(0, UITheme.BottomNavHeight);

            var grid = nav.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(200, 140);
            grid.spacing = new Vector2(8, 8);
            grid.padding = new RectOffset(12, 12, 12, 12);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 5;

            var tabs = new UIBottomNav.NavTab[5];
            tabs[0] = CreateNavTab(nav, "Home", UIScreenId.Dashboard, UITheme.AccentPrimary);
            tabs[1] = CreateNavTab(nav, "Biz", UIScreenId.BusinessList, UITheme.AccentSecondary);
            tabs[2] = CreateNavTab(nav, "Staff", UIScreenId.EmployeeHiring, UITheme.AccentWarning);
            tabs[3] = CreateNavTab(nav, "Shop", UIScreenId.Shop, UITheme.AccentPremium);
            tabs[4] = CreateNavTab(nav, "More", UIScreenId.Settings, UITheme.BackgroundElevated);

            var bottom = nav.gameObject.AddComponent<UIBottomNav>();
            bottom.Setup(tabs);
            return bottom;
        }

        private static UIBottomNav.NavTab CreateNavTab(RectTransform parent, string label, UIScreenId id, Color color)
        {
            var icon = RuntimeUIFactory.CreatePanel(parent, label + "Icon", color, false);
            icon.sizeDelta = new Vector2(80, 80);
            var btn = icon.gameObject.AddComponent<Button>();
            var text = RuntimeUIFactory.CreateText(icon, "Label", label, 22, UITheme.TextSecondary);
            text.rectTransform.anchorMin = new Vector2(0, 0);
            text.rectTransform.anchorMax = new Vector2(1, 0.35f);
            return new UIBottomNav.NavTab
            {
                screenId = id,
                button = btn,
                iconPlaceholder = icon.GetComponent<Image>(),
                label = text,
                activeColor = color
            };
        }

        private static void BuildDashboard(RectTransform root, out Button buyBtn, out Button upgradeBtn, out Button earnBtn)
        {
            var panel = CreateScreenPanel(root, "Dashboard");
            var screen = panel.gameObject.AddComponent<DashboardScreen>();

            var welcome = RuntimeUIFactory.CreateText(panel, "Welcome", "Welcome, CEO", 40, UITheme.TextPrimary);
            welcome.rectTransform.anchorMin = new Vector2(0, 0.85f);
            welcome.rectTransform.anchorMax = new Vector2(1, 0.95f);

            var cards = new UICardView[4];
            for (int i = 0; i < 4; i++)
            {
                var cardRt = RuntimeUIFactory.CreatePanel(panel, "Card" + i, UITheme.BackgroundElevated, false);
                cardRt.anchorMin = new Vector2(0.05f + (i % 2) * 0.48f, i < 2 ? 0.52f : 0.28f);
                cardRt.anchorMax = new Vector2(0.05f + (i % 2) * 0.48f + 0.44f, i < 2 ? 0.78f : 0.54f);
                cardRt.offsetMin = Vector2.zero;
                cardRt.offsetMax = Vector2.zero;
                var accent = RuntimeUIFactory.CreatePanel(cardRt, "Accent", UITheme.AccentPrimary, false);
                accent.anchorMin = new Vector2(0, 0);
                accent.anchorMax = new Vector2(0, 1);
                accent.sizeDelta = new Vector2(8, 0);
                var title = RuntimeUIFactory.CreateText(cardRt, "Title", "Stat", 24, UITheme.TextSecondary, TextAlignmentOptions.TopLeft);
                title.rectTransform.offsetMin = new Vector2(16, 0);
                var val = RuntimeUIFactory.CreateText(cardRt, "Value", "0", 36, UITheme.TextPrimary, TextAlignmentOptions.TopLeft);
                val.rectTransform.offsetMin = new Vector2(16, -40);
                var sub = RuntimeUIFactory.CreateText(cardRt, "Sub", "", 22, UITheme.TextMuted, TextAlignmentOptions.TopLeft);
                sub.rectTransform.offsetMin = new Vector2(16, -80);
                var card = cardRt.gameObject.AddComponent<UICardView>();
                card.Setup(title, val, sub, accent.GetComponent<Image>(), accent.GetComponent<Image>());
                cards[i] = card;
            }

            buyBtn = RuntimeUIFactory.CreateButton(panel, "BuyBusinessButton", "Buy Business", UITheme.AccentSuccess, 72);
            buyBtn.GetComponent<RectTransform>().anchorMin = new Vector2(0.05f, 0.08f);
            buyBtn.GetComponent<RectTransform>().anchorMax = new Vector2(0.35f, 0.16f);
            buyBtn.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            buyBtn.GetComponent<RectTransform>().offsetMax = Vector2.zero;

            upgradeBtn = RuntimeUIFactory.CreateButton(panel, "UpgradeBusinessButton", "Upgrade Business", UITheme.AccentPrimary, 72);
            upgradeBtn.GetComponent<RectTransform>().anchorMin = new Vector2(0.37f, 0.08f);
            upgradeBtn.GetComponent<RectTransform>().anchorMax = new Vector2(0.63f, 0.16f);
            upgradeBtn.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            upgradeBtn.GetComponent<RectTransform>().offsetMax = Vector2.zero;

            earnBtn = RuntimeUIFactory.CreateButton(panel, "EarnMoneyButton", "Earn Money (Test)", UITheme.AccentWarning, 72);
            earnBtn.GetComponent<RectTransform>().anchorMin = new Vector2(0.65f, 0.08f);
            earnBtn.GetComponent<RectTransform>().anchorMax = new Vector2(0.95f, 0.16f);
            earnBtn.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            earnBtn.GetComponent<RectTransform>().offsetMax = Vector2.zero;

            WireDashboard(screen, welcome, cards);
            panel.gameObject.SetActive(true);
        }

        private static void BuildBusinessList(RectTransform root, UIListRowView rowTemplate)
        {
            var panel = CreateScreenPanel(root, "BusinessList");
            var scroll = RuntimeUIFactory.CreateScrollView(panel, "Scroll", out var content);
            var screen = panel.gameObject.AddComponent<BusinessListScreen>();
            WireListScreen(screen, content, rowTemplate);
            panel.gameObject.SetActive(false);
        }

        private static void BuildEmployeeScreen(RectTransform root, UIListRowView rowTemplate)
        {
            var panel = CreateScreenPanel(root, "EmployeeHiring");
            var summary = RuntimeUIFactory.CreateText(panel, "Summary", "Employees", 28, UITheme.AccentWarning);
            summary.rectTransform.anchorMin = new Vector2(0, 0.92f);
            summary.rectTransform.anchorMax = new Vector2(1, 0.98f);
            var scroll = RuntimeUIFactory.CreateScrollView(panel, "Scroll", out var content);
            scroll.GetComponent<RectTransform>().offsetMax = new Vector2(-16, -120);
            var screen = panel.gameObject.AddComponent<EmployeeHiringScreen>();
            WireEmployeeScreen(screen, content, rowTemplate, summary);
            panel.gameObject.SetActive(false);
        }

        private static void BuildShop(RectTransform root, UIListRowView rowTemplate)
        {
            var panel = CreateScreenPanel(root, "Shop");
            var gems = RuntimeUIFactory.CreateText(panel, "Gems", "Gems: 0", 32, UITheme.AccentPremium);
            gems.rectTransform.anchorMin = new Vector2(0, 0.92f);
            gems.rectTransform.anchorMax = new Vector2(1, 0.98f);
            var scroll = RuntimeUIFactory.CreateScrollView(panel, "Scroll", out var content);
            scroll.GetComponent<RectTransform>().offsetMax = new Vector2(-16, -100);
            var screen = panel.gameObject.AddComponent<ShopScreen>();
            WireShopScreen(screen, content, rowTemplate, gems);
            panel.gameObject.SetActive(false);
        }

        private static void BuildSettings(RectTransform root)
        {
            var panel = CreateScreenPanel(root, "Settings");
            RuntimeUIFactory.CreateText(panel, "Header", "Settings", 44, UITheme.TextPrimary).rectTransform.anchoredPosition = new Vector2(0, 700);
            var reset = RuntimeUIFactory.CreateButton(panel, "Reset", "Reset Save", UITheme.AccentDanger, 80);
            reset.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -600);
            var back = RuntimeUIFactory.CreateButton(panel, "Back", "Back", UITheme.BackgroundElevated, 80);
            back.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -720);
            var screen = panel.gameObject.AddComponent<SettingsScreen>();
            WireSettingsScreen(screen, back, reset);
            panel.gameObject.SetActive(false);
        }

        private static void BuildBusinessDetail(RectTransform root)
        {
            var panel = CreateScreenPanel(root, "BusinessDetail");
            var title = RuntimeUIFactory.CreateText(panel, "Title", "Business", 40, UITheme.TextPrimary);
            title.rectTransform.anchoredPosition = new Vector2(0, 650);
            var desc = RuntimeUIFactory.CreateText(panel, "Desc", "", 26, UITheme.TextSecondary);
            desc.rectTransform.sizeDelta = new Vector2(900, 200);
            desc.rectTransform.anchoredPosition = new Vector2(0, 450);
            var stats = RuntimeUIFactory.CreateText(panel, "Stats", "", 28, UITheme.TextPrimary, TextAlignmentOptions.TopLeft);
            stats.rectTransform.sizeDelta = new Vector2(900, 300);
            stats.rectTransform.anchoredPosition = new Vector2(0, 150);
            var upgrade = RuntimeUIFactory.CreateButton(panel, "Upgrade", "Upgrade", UITheme.AccentPrimary);
            upgrade.GetComponent<RectTransform>().anchoredPosition = new Vector2(-150, -350);
            var hire = RuntimeUIFactory.CreateButton(panel, "Hire", "Hire Staff", UITheme.AccentWarning);
            hire.GetComponent<RectTransform>().anchoredPosition = new Vector2(150, -350);
            var back = RuntimeUIFactory.CreateButton(panel, "Back", "Back", UITheme.BackgroundElevated);
            back.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -500);
            var icon = RuntimeUIFactory.CreatePanel(panel, "Icon", UITheme.AccentSecondary, false);
            icon.sizeDelta = new Vector2(120, 120);
            icon.anchoredPosition = new Vector2(0, 780);
            var screen = panel.gameObject.AddComponent<BusinessDetailScreen>();
            WireBusinessDetail(screen, title, desc, stats, icon.GetComponent<Image>(), upgrade, hire, back);
            panel.gameObject.SetActive(false);
        }

        private static void BuildBuyBusiness(RectTransform root)
        {
            var panel = CreateScreenPanel(root, "BuyBusiness");
            var title = RuntimeUIFactory.CreateText(panel, "Title", "Buy Business", 40, UITheme.TextPrimary);
            title.rectTransform.anchoredPosition = new Vector2(0, 600);
            var cost = RuntimeUIFactory.CreateText(panel, "Cost", "$0", 34, UITheme.AccentWarning);
            cost.rectTransform.anchoredPosition = new Vector2(0, 400);
            var income = RuntimeUIFactory.CreateText(panel, "Income", "", 28, UITheme.AccentSuccess);
            income.rectTransform.anchoredPosition = new Vector2(0, 300);
            var desc = RuntimeUIFactory.CreateText(panel, "Desc", "", 26, UITheme.TextSecondary);
            desc.rectTransform.sizeDelta = new Vector2(900, 200);
            desc.rectTransform.anchoredPosition = new Vector2(0, 100);
            var buy = RuntimeUIFactory.CreateButton(panel, "Buy", "Buy Business", UITheme.AccentSuccess);
            buy.GetComponent<RectTransform>().anchoredPosition = new Vector2(-150, -300);
            var cancel = RuntimeUIFactory.CreateButton(panel, "Cancel", "Cancel", UITheme.BackgroundElevated);
            cancel.GetComponent<RectTransform>().anchoredPosition = new Vector2(150, -300);
            var screen = panel.gameObject.AddComponent<BuyBusinessScreen>();
            WireBuyBusiness(screen, title, cost, income, desc, buy, cancel);
            panel.gameObject.SetActive(false);
        }

        private static void BuildUpgradeBusiness(RectTransform root)
        {
            var panel = CreateScreenPanel(root, "UpgradeBusiness");
            var title = RuntimeUIFactory.CreateText(panel, "Title", "Upgrade", 40, UITheme.TextPrimary);
            title.rectTransform.anchoredPosition = new Vector2(0, 600);
            var level = RuntimeUIFactory.CreateText(panel, "Level", "Lv 1", 30, UITheme.TextSecondary);
            level.rectTransform.anchoredPosition = new Vector2(0, 450);
            var cost = RuntimeUIFactory.CreateText(panel, "Cost", "$0", 32, UITheme.AccentWarning);
            cost.rectTransform.anchoredPosition = new Vector2(0, 350);
            var bonus = RuntimeUIFactory.CreateText(panel, "Bonus", "+15%", 28, UITheme.AccentSuccess);
            bonus.rectTransform.anchoredPosition = new Vector2(0, 250);
            var upgrade = RuntimeUIFactory.CreateButton(panel, "Upgrade", "Upgrade", UITheme.AccentPrimary);
            upgrade.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);
            var back = RuntimeUIFactory.CreateButton(panel, "Back", "Back", UITheme.BackgroundElevated);
            back.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -350);
            var screen = panel.gameObject.AddComponent<UpgradeBusinessScreen>();
            WireUpgradeBusiness(screen, title, level, cost, bonus, upgrade, back);
            panel.gameObject.SetActive(false);
        }

        private static RectTransform CreateStretchChild(Transform parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            RuntimeUIFactory.Stretch(rt);
            return rt;
        }

        private static RectTransform CreateScreenPanel(RectTransform parent, string name)
        {
            return RuntimeUIFactory.CreatePanel(parent, name, new Color(0, 0, 0, 0));
        }

        private static void WireDashboard(DashboardScreen s, TextMeshProUGUI welcome, UICardView[] cards)
        {
            SetField(s, "welcomeText", welcome);
            SetField(s, "statCards", cards);
        }

        private static void WireListScreen(BusinessListScreen s, RectTransform list, UIListRowView prefab)
        {
            SetField(s, "listContainer", list);
            SetField(s, "rowPrefab", prefab);
        }

        private static void WireEmployeeScreen(EmployeeHiringScreen s, RectTransform list, UIListRowView prefab, TextMeshProUGUI summary)
        {
            SetField(s, "listContainer", list);
            SetField(s, "rowPrefab", prefab);
            SetField(s, "summaryText", summary);
        }

        private static void WireShopScreen(ShopScreen s, RectTransform list, UIListRowView prefab, TextMeshProUGUI gems)
        {
            SetField(s, "listContainer", list);
            SetField(s, "rowPrefab", prefab);
            SetField(s, "gemsBalanceText", gems);
        }

        private static void WireSettingsScreen(SettingsScreen s, Button back, Button reset)
        {
            SetField(s, "backButton", back);
            SetField(s, "resetButton", reset);
        }

        private static void WireBusinessDetail(BusinessDetailScreen s, TextMeshProUGUI title, TextMeshProUGUI desc,
            TextMeshProUGUI stats, Image icon, Button upgrade, Button hire, Button back)
        {
            SetField(s, "titleText", title);
            SetField(s, "descText", desc);
            SetField(s, "statsText", stats);
            SetField(s, "iconPlaceholder", icon);
            SetField(s, "upgradeButton", upgrade);
            SetField(s, "hireButton", hire);
            SetField(s, "backButton", back);
        }

        private static void WireBuyBusiness(BuyBusinessScreen s, TextMeshProUGUI title, TextMeshProUGUI cost,
            TextMeshProUGUI income, TextMeshProUGUI desc, Button buy, Button cancel)
        {
            SetField(s, "titleText", title);
            SetField(s, "costText", cost);
            SetField(s, "incomeText", income);
            SetField(s, "descText", desc);
            SetField(s, "buyButton", buy);
            SetField(s, "cancelButton", cancel);
        }

        private static void WireUpgradeBusiness(UpgradeBusinessScreen s, TextMeshProUGUI title, TextMeshProUGUI level,
            TextMeshProUGUI cost, TextMeshProUGUI bonus, Button upgrade, Button back)
        {
            SetField(s, "titleText", title);
            SetField(s, "currentLevelText", level);
            SetField(s, "upgradeCostText", cost);
            SetField(s, "bonusText", bonus);
            SetField(s, "upgradeButton", upgrade);
            SetField(s, "backButton", back);
        }

        private static void SetField(object target, string field, object value)
        {
            var f = target.GetType().GetField(field,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            f?.SetValue(target, value);
        }
    }
}
