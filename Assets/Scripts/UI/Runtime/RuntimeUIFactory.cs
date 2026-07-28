using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Runtime
{
    /// <summary>
    /// Runtime UI factory — builds placeholder mobile UI without editor tools or external assets.
    /// </summary>
    public static class RuntimeUIFactory
    {
        private static TMP_FontAsset _font;

        public static TMP_FontAsset Font
        {
            get
            {
                if (_font == null)
                    _font = TMP_Settings.defaultFontAsset;
                return _font;
            }
        }

        public static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<EventSystem>() != null) return;
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        public static Canvas CreateCanvas(string name)
        {
            var go = new GameObject(name);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(UITheme.ReferenceWidth, UITheme.ReferenceHeight);
            scaler.matchWidthOrHeight = 0.5f;
            go.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        public static RectTransform CreatePanel(Transform parent, string name, Color color, bool stretch = true)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            var rt = go.GetComponent<RectTransform>();
            if (stretch) Stretch(rt);
            return rt;
        }

        public static TextMeshProUGUI CreateText(Transform parent, string name, string text, float size, Color color,
            TextAlignmentOptions align = TextAlignmentOptions.Center)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.alignment = align;
            if (Font != null) tmp.font = Font;
            tmp.raycastTarget = false;
            return tmp;
        }

        public static Button CreateButton(Transform parent, string name, string label, Color bg, float height = 96f)
        {
            var rt = CreatePanel(parent, name, bg, false);
            rt.sizeDelta = new Vector2(400, height);
            var text = CreateText(rt, "Label", label, 32, UITheme.TextPrimary);
            Stretch(text.rectTransform);
            var btn = rt.gameObject.AddComponent<Button>();
            rt.gameObject.AddComponent<UIButtonFeedback>();
            return btn;
        }

        public static ScrollRect CreateScrollView(Transform parent, string name, out RectTransform content)
        {
            var scrollRt = CreatePanel(parent, name, new Color(0, 0, 0, 0));
            Stretch(scrollRt);
            scrollRt.offsetMin = new Vector2(16, 16);
            scrollRt.offsetMax = new Vector2(-16, -16);

            var viewport = CreatePanel(scrollRt, "Viewport", new Color(0, 0, 0, 0));
            Stretch(viewport);
            viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;

            var contentGo = new GameObject("Content");
            contentGo.transform.SetParent(viewport, false);
            content = contentGo.AddComponent<RectTransform>();
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
            content.pivot = new Vector2(0.5f, 1);
            content.sizeDelta = new Vector2(0, 0);

            var layout = contentGo.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 12;
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;
            contentGo.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scroll = scrollRt.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            return scroll;
        }

        public static UIListRowView CreateListRowPrefab(Transform parent)
        {
            var rt = CreatePanel(parent, "ListRowTemplate", UITheme.BackgroundElevated, false);
            rt.sizeDelta = new Vector2(900, 120);
            var le = rt.gameObject.AddComponent<LayoutElement>();
            le.minHeight = 120;
            le.preferredHeight = 120;

            var icon = CreatePanel(rt, "Icon", UITheme.AccentPrimary, false);
            icon.anchorMin = new Vector2(0, 0.5f);
            icon.anchorMax = new Vector2(0, 0.5f);
            icon.sizeDelta = new Vector2(72, 72);
            icon.anchoredPosition = new Vector2(48, 0);

            var title = CreateText(rt, "Title", "Title", 30, UITheme.TextPrimary, TextAlignmentOptions.Left);
            title.rectTransform.anchorMin = new Vector2(0, 0.55f);
            title.rectTransform.anchorMax = new Vector2(0.7f, 1);
            title.rectTransform.offsetMin = new Vector2(100, 0);

            var sub = CreateText(rt, "Subtitle", "Subtitle", 24, UITheme.TextSecondary, TextAlignmentOptions.Left);
            sub.rectTransform.anchorMin = new Vector2(0, 0);
            sub.rectTransform.anchorMax = new Vector2(0.7f, 0.55f);
            sub.rectTransform.offsetMin = new Vector2(100, 0);

            var val = CreateText(rt, "Value", "$0", 26, UITheme.AccentSuccess, TextAlignmentOptions.Right);
            val.rectTransform.anchorMin = new Vector2(0.55f, 0.2f);
            val.rectTransform.anchorMax = new Vector2(1, 0.8f);
            val.rectTransform.offsetMax = new Vector2(-140, 0);

            var action = CreateButton(rt, "Action", "Open", UITheme.AccentPrimary, 72);
            action.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
            action.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
            action.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 72);
            action.GetComponent<RectTransform>().anchoredPosition = new Vector2(-70, 0);

            var row = rt.gameObject.AddComponent<UIListRowView>();
            row.Setup(title, sub, val, icon.GetComponent<Image>(), action);

            rt.gameObject.SetActive(false);
            return row;
        }

        public static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
