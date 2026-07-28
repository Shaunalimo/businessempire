using UnityEngine;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Premium blue-green corporate theme for Business Empire mobile UI.
    /// </summary>
    public static class UITheme
    {
        // Corporate backgrounds
        public static readonly Color BackgroundDark = new Color(0.04f, 0.09f, 0.14f, 1f);
        public static readonly Color BackgroundCard = new Color(0.08f, 0.14f, 0.20f, 1f);
        public static readonly Color BackgroundElevated = new Color(0.10f, 0.18f, 0.26f, 1f);
        public static readonly Color BackgroundGradientTop = new Color(0.06f, 0.16f, 0.24f, 1f);
        public static readonly Color OverlayDim = new Color(0.02f, 0.06f, 0.10f, 0.75f);

        // Brand accents — blue-green corporate
        public static readonly Color AccentPrimary = new Color(0.12f, 0.72f, 0.68f, 1f);    // Teal
        public static readonly Color AccentSecondary = new Color(0.18f, 0.52f, 0.92f, 1f);  // Corporate blue
        public static readonly Color AccentSuccess = new Color(0.20f, 0.82f, 0.55f, 1f);
        public static readonly Color AccentWarning = new Color(0.95f, 0.78f, 0.22f, 1f);
        public static readonly Color AccentDanger = new Color(0.92f, 0.32f, 0.38f, 1f);
        public static readonly Color AccentPremium = new Color(0.45f, 0.85f, 0.95f, 1f);

        // Text
        public static readonly Color TextPrimary = new Color(0.96f, 0.98f, 1f, 1f);
        public static readonly Color TextSecondary = new Color(0.62f, 0.74f, 0.82f, 1f);
        public static readonly Color TextMuted = new Color(0.40f, 0.52f, 0.60f, 1f);
        public static readonly Color TextAccent = new Color(0.35f, 0.95f, 0.88f, 1f);

        // Layout — portrait Android reference
        public const float ReferenceWidth = 1080f;
        public const float ReferenceHeight = 1920f;
        public const float TopBarHeight = 168f;
        public const float BottomNavHeight = 168f;
        public const float SafeAreaPadding = 24f;
        public const float PanelPadding = 32f;
        public const float CardRadius = 20f;
        public const float ButtonHeight = 96f;
        public const float NavButtonSize = 112f;
    }
}
