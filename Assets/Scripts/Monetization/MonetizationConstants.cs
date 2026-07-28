namespace BusinessEmpire.Monetization
{
    /// <summary>
    /// Google AdMob TEST ad unit IDs — safe for development builds only.
    /// Replace with production IDs before release (see Assets/Documentation/MONETIZATION.md).
    /// </summary>
    public static class MonetizationConstants
    {
        // ── AdMob App ID (Android test) ──────────────────────────────────────
        public const string AdMobAppIdAndroid = "ca-app-pub-3940256099942544~3347511713";

        // ── AdMob TEST ad units (Google official sample IDs) ─────────────────
        public const string AdMobBannerAndroid = "ca-app-pub-3940256099942544/6300978111";
        public const string AdMobInterstitialAndroid = "ca-app-pub-3940256099942544/1033173712";
        public const string AdMobRewardedAndroid = "ca-app-pub-3940256099942544/5224354917";

        // iOS test placeholders (for future expansion)
        public const string AdMobBannerIos = "ca-app-pub-3940256099942544/2934735716";
        public const string AdMobInterstitialIos = "ca-app-pub-3940256099942544/4411468910";
        public const string AdMobRewardedIos = "ca-app-pub-3940256099942544/1712485313";

        // ── Google Play product IDs (create matching products in Play Console) ─
        public const string ProductRemoveAds = "com.tekhgames.businessempire.removeads";
        public const string ProductGems100 = "com.tekhgames.businessempire.gems100";
        public const string ProductGems500 = "com.tekhgames.businessempire.gems500";
        public const string ProductGems1200 = "com.tekhgames.businessempire.gems1200";
        public const string ProductStarterPack = "com.tekhgames.businessempire.starterpack";

        // ── Rewarded ad payout tuning (game economy) ─────────────────────────
        public const double ExtraCashReward = 5000d;
        public const float OfflineEarningsMultiplier = 2f;
        public const float DailyRewardAdMultiplier = 2f;
        public const int StarterPackGems = 250;
        public const double StarterPackCash = 25000d;

        // ── Interstitial frequency guards ────────────────────────────────────
        public const float MinSecondsBetweenInterstitials = 180f;
        public const int DaysBetweenWeeklyInterstitial = 7;
        public const int UpgradeLevelMilestoneInterval = 5;
    }
}
