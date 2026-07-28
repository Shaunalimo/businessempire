#if UNITY_EDITOR
using BusinessEmpire.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.Editor
{
    /// <summary>
    /// Factory for creating styled mobile UI elements in the editor.
    /// </summary>
    public static class UIFactory
    {
        private static TMP_FontAsset _font;

        public static TMP_FontAsset DefaultFont
        {
            get
            {
                if (_font == null)
                {
                    _font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                        "Packages/com.unity.textmeshpro/Resources/Fonts & Materials/LiberationSans SDF.asset");
                }
                return _font;
            }
        }

        public static GameObject CreateCanvas(string name)
        {
            var go = new GameObject(name);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(UITheme.ReferenceWidth, UITheme.ReferenceHeight);
            scaler.matchWidthOrHeight = 0.5f;
            go.AddComponent<GraphicRaycaster>();
            return go;
        }

        public static GameObject CreatePanel(Transform parent, string name, Color color, bool stretch = true)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            var rt = go.GetComponent<RectTransform>();
            if (stretch) StretchFull(rt);
            return go;
        }

        public static TextMeshProUGUI CreateText(Transform parent, string name, string text, float size,
            Color color, TextAlignmentOptions align = TextAlignmentOptions.Center)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.alignment = align;
            if (DefaultFont != null) tmp.font = DefaultFont;
            tmp.raycastTarget = false;
            return tmp;
        }

        public static Button CreateButton(Transform parent, string name, string label, Color bgColor,
            float height = UITheme.ButtonHeight)
        {
            var go = CreatePanel(parent, name, bgColor, stretch: false);
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(400, height);

            var text = CreateText(go.transform, "Label", label, 36, UITheme.TextPrimary);
            StretchFull(text.rectTransform);

            var btn = go.AddComponent<Button>();
            var colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.1f, 1.1f, 1.1f);
            colors.pressedColor = new Color(0.85f, 0.85f, 0.85f);
            btn.colors = colors;
            go.AddComponent<UIButtonFeedback>();
            return btn;
        }

        public static GameObject CreatePopupShell(Transform parent, string name, out UIAnimator animator,
            out Button closeButton)
        {
            // Full-screen overlay
            var overlay = CreatePanel(parent, name, UITheme.OverlayDim);
            StretchFull(overlay.GetComponent<RectTransform>());
            overlay.SetActive(false);

            var panel = CreatePanel(overlay.transform, "Panel", UITheme.BackgroundCard, stretch: false);
            var panelRt = panel.GetComponent<RectTransform>();
            panelRt.anchorMin = new Vector2(0.05f, 0.12f);
            panelRt.anchorMax = new Vector2(0.95f, 0.88f);
            panelRt.offsetMin = Vector2.zero;
            panelRt.offsetMax = Vector2.zero;

            var cg = panel.AddComponent<CanvasGroup>();
            animator = panel.AddComponent<UIAnimator>();

            // Wire animator fields via SerializedObject
            var so = new SerializedObject(animator);
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.FindProperty("panelTransform").objectReferenceValue = panelRt;
            so.ApplyModifiedPropertiesWithoutUndo();

            var header = CreateText(panel.transform, "Title", name, 48, UITheme.TextPrimary,
                TextAlignmentOptions.Top);
            var headerRt = header.rectTransform;
            headerRt.anchorMin = new Vector2(0, 1);
            headerRt.anchorMax = new Vector2(1, 1);
            headerRt.pivot = new Vector2(0.5f, 1);
            headerRt.sizeDelta = new Vector2(0, 80);
            headerRt.anchoredPosition = new Vector2(0, -16);

            closeButton = CreateButton(panel.transform, "CloseButton", "✕", UITheme.AccentDanger, 64);
            var closeRt = closeButton.GetComponent<RectTransform>();
            closeRt.anchorMin = new Vector2(1, 1);
            closeRt.anchorMax = new Vector2(1, 1);
            closeRt.pivot = new Vector2(1, 1);
            closeRt.anchoredPosition = new Vector2(-16, -16);
            closeRt.sizeDelta = new Vector2(64, 64);

            return overlay;
        }

        public static ScrollRect CreateScrollView(Transform parent, string name, out Transform content)
        {
            var scrollGo = CreatePanel(parent, name, new Color(0, 0, 0, 0), stretch: true);
            var scrollRt = scrollGo.GetComponent<RectTransform>();
            scrollRt.anchorMin = new Vector2(0, 0);
            scrollRt.anchorMax = new Vector2(1, 1);
            scrollRt.offsetMin = new Vector2(16, 16);
            scrollRt.offsetMax = new Vector2(-16, -100);

            var scroll = scrollGo.AddComponent<ScrollRect>();
            scroll.horizontal = false;

            var viewport = CreatePanel(scrollGo.transform, "Viewport", new Color(0, 0, 0, 0));
            StretchFull(viewport.GetComponent<RectTransform>());
            viewport.AddComponent<Mask>().showMaskGraphic = false;
            var viewportImg = viewport.GetComponent<Image>();
            viewportImg.color = new Color(1, 1, 1, 0.01f);

            var contentGo = CreatePanel(viewport.transform, "Content", new Color(0, 0, 0, 0), stretch: false);
            var contentRt = contentGo.GetComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0, 1);
            contentRt.anchorMax = new Vector2(1, 1);
            contentRt.pivot = new Vector2(0.5f, 1);
            contentRt.sizeDelta = new Vector2(0, 0);

            var layout = contentGo.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 16;
            layout.padding = new RectOffset(8, 8, 8, 8);
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;
            contentGo.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll.viewport = viewport.GetComponent<RectTransform>();
            scroll.content = contentRt;
            content = contentGo.transform;
            return scroll;
        }

        public static Slider CreateSlider(Transform parent, string name, string label)
        {
            var row = new GameObject(name);
            row.transform.SetParent(parent, false);
            row.AddComponent<LayoutElement>().minHeight = 80;

            CreateText(row.transform, "Label", label, 28, UITheme.TextSecondary, TextAlignmentOptions.Left);

            var sliderGo = new GameObject("Slider");
            sliderGo.transform.SetParent(row.transform, false);
            var slider = sliderGo.AddComponent<Slider>();
            slider.minValue = 0;
            slider.maxValue = 1;
            slider.value = 0.8f;

            var bg = CreatePanel(sliderGo.transform, "Background", UITheme.BackgroundElevated, stretch: false);
            bg.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 12);
            var fill = CreatePanel(sliderGo.transform, "Fill", UITheme.AccentPrimary, stretch: false);
            var fillRt = fill.GetComponent<RectTransform>();
            slider.fillRect = fillRt;
            slider.targetGraphic = fill.GetComponent<Image>();

            return slider;
        }

        public static Toggle CreateToggle(Transform parent, string name, string label)
        {
            var row = new GameObject(name);
            row.transform.SetParent(parent, false);
            row.AddComponent<LayoutElement>().minHeight = 64;
            CreateText(row.transform, "Label", label, 28, UITheme.TextSecondary, TextAlignmentOptions.Left);

            var toggleGo = new GameObject("Toggle");
            toggleGo.transform.SetParent(row.transform, false);
            var toggle = toggleGo.AddComponent<Toggle>();
            var bg = CreatePanel(toggleGo.transform, "Background", UITheme.BackgroundElevated, stretch: false);
            bg.GetComponent<RectTransform>().sizeDelta = new Vector2(48, 48);
            toggle.targetGraphic = bg.GetComponent<Image>();
            return toggle;
        }

        public static void StretchFull(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        public static void SetSerializedField(Object target, string fieldName, Object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop != null)
            {
                prop.objectReferenceValue = value;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        public static void SetSerializedField(Object target, string fieldName, bool value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop != null)
            {
                prop.boolValue = value;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        public static void SetSerializedField<T>(Object target, string fieldName, T[] values) where T : Object
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop == null || !prop.isArray)
                return;

            prop.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
                prop.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
#endif
