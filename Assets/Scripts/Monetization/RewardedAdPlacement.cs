namespace BusinessEmpire.Monetization
{
    /// <summary>
    /// Identifies optional rewarded ad placements. Player always opts in explicitly.
    /// </summary>
    public enum RewardedAdPlacement
    {
        DoubleOfflineEarnings,
        ExtraCash,
        InstantUpgradeSpeedUp,
        DailyRewardMultiplier,
        GenericTest
    }

    /// <summary>
    /// Major milestones that may trigger an interstitial (never on every click).
    /// </summary>
    public enum InterstitialMilestone
    {
        BusinessPurchased,
        BusinessUpgraded,
        AchievementUnlocked,
        WeeklyProgress,
        GenericTest
    }
}
