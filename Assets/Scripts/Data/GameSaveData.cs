using System;
using System.Collections.Generic;

namespace BusinessEmpire.Data
{
    /// <summary>
    /// Complete persistent game state serialized to JSON.
    /// </summary>
    [Serializable]
    public class GameSaveData
    {
        public int saveVersion = 1;
        public string playerName = "CEO";
        public double cash;
        public int premiumCurrency;
        public int playerLevel = 1;
        public double playerXp;
        public long lastSaveTimestamp;
        public long lastOnlineTimestamp;
        public int totalDaysPlayed;
        public int consecutiveLoginDays;
        public long lastDailyRewardClaimTimestamp;
        public int dailyRewardIndex;
        public bool adsRemoved;
        public bool cloudSaveEnabled;

        public List<BusinessInstanceData> businesses = new List<BusinessInstanceData>();
        public List<LoanData> activeLoans = new List<LoanData>();
        public List<InvestmentData> investments = new List<InvestmentData>();
        public List<MarketingCampaignData> activeCampaigns = new List<MarketingCampaignData>();
        public List<ResearchData> completedResearch = new List<ResearchData>();
        public List<string> unlockedAchievements = new List<string>();
        public List<string> pendingEvents = new List<string>();
        public GameSettingsData settings = new GameSettingsData();

        public double totalLifetimeProfit;
        public int businessesOwned;
        public int totalEmployeesHired;
        public int reputation = 50;
    }

    [Serializable]
    public class LoanData
    {
        public string loanId;
        public double principal;
        public double remainingBalance;
        public double dailyInterestRate;
        public int daysRemaining;
        public long takenTimestamp;
    }

    [Serializable]
    public class InvestmentData
    {
        public string investmentId;
        public string name;
        public double amountInvested;
        public double expectedReturnRate;
        public int daysUntilMaturity;
        public long investedTimestamp;
    }

    [Serializable]
    public class MarketingCampaignData
    {
        public string campaignId;
        public BusinessType targetBusiness;
        public double incomeBonusMultiplier = 1.2d;
        public int daysRemaining;
    }

    [Serializable]
    public class ResearchData
    {
        public string researchId;
        public string name;
        public bool isCompleted;
        public double globalIncomeBonus;
    }

    [Serializable]
    public class GameSettingsData
    {
        public float masterVolume = 0.8f;
        public float sfxVolume = 1f;
        public bool notificationsEnabled = true;
        public bool hapticFeedback = true;
        public string languageCode = "en";
    }
}
