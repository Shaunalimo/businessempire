using BusinessEmpire.Monetization;

namespace BusinessEmpire.Core
{
    /// <summary>
    /// Global constants used across the game.
    /// </summary>
    public static class GameConstants
    {
        public const string GameTitle = "Business Empire: CEO Simulator";
        public const double StartingCash = 5000d;
        public const float AutoSaveIntervalSeconds = 30f;
        public const float DayDurationSeconds = 60f;
        public const int MaxBusinessLevel = 10;
        public const int MaxEmployeesPerBusiness = 50;
        public const double OfflineIncomeCapHours = 24d;
        public const string SaveFileName = "business_empire_save.json";
        public const string PremiumCurrencyId = "Gems";
        public const int DailyRewardCycleDays = 7;
        public const string RemoveAdsProductId = MonetizationConstants.ProductRemoveAds;
        public const string GemPackSmallProductId = MonetizationConstants.ProductGems100;
        public const string GemPackMediumProductId = MonetizationConstants.ProductGems500;
        public const string GemPackLargeProductId = MonetizationConstants.ProductGems1200;

        // AdMob IDs: see MonetizationConstants (TEST IDs until production).
        public const string AdMobAppIdAndroid = MonetizationConstants.AdMobAppIdAndroid;
        public const string AdMobRewardedAndroid = MonetizationConstants.AdMobRewardedAndroid;
        public const string AdMobInterstitialAndroid = MonetizationConstants.AdMobInterstitialAndroid;
        public const string AdMobRewardedIos = MonetizationConstants.AdMobRewardedIos;
        public const string AdMobInterstitialIos = MonetizationConstants.AdMobInterstitialIos;
    }
}
