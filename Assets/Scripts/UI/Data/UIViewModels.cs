using System;
using System.Collections.Generic;
using BusinessEmpire.Data;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// View models for UI display — decoupled from game economy logic.
    /// </summary>
    public static class UIViewModels
    {
        [Serializable]
        public struct PlayerHeader
        {
            public string playerName;
            public double cash;
            public int gems;
            public int companyLevel;
            public int reputation;
        }

        [Serializable]
        public struct BusinessCard
        {
            public BusinessType type;
            public string name;
            public string description;
            public bool isOwned;
            public int level;
            public int employees;
            public double purchaseCost;
            public double dailyProfit;
            public double dailyIncome;
            public string iconKey;
        }

        [Serializable]
        public struct EmployeeSlot
        {
            public BusinessType businessType;
            public string businessName;
            public int current;
            public int max;
            public double hireCost;
            public double salaryPerDay;
        }

        [Serializable]
        public struct MarketingOption
        {
            public string id;
            public string name;
            public string description;
            public double cost;
            public string bonusText;
            public int durationDays;
        }

        [Serializable]
        public struct LoanOption
        {
            public string id;
            public string name;
            public double amount;
            public string terms;
        }

        [Serializable]
        public struct InvestmentOption
        {
            public string id;
            public string name;
            public double minAmount;
            public string returnText;
            public int days;
        }

        [Serializable]
        public struct ReportSummary
        {
            public double totalRevenue;
            public double totalExpenses;
            public double netProfit;
            public int daysPlayed;
            public List<BusinessProfitLine> lines;
        }

        [Serializable]
        public struct BusinessProfitLine
        {
            public string name;
            public double profit;
        }

        [Serializable]
        public struct DailyRewardDay
        {
            public int day;
            public double cash;
            public int gems;
            public bool isClaimed;
            public bool isToday;
        }

        [Serializable]
        public struct AchievementItem
        {
            public string id;
            public string title;
            public string description;
            public bool unlocked;
            public int gemReward;
        }

        [Serializable]
        public struct ShopProduct
        {
            public string id;
            public string name;
            public string description;
            public string price;
            public int gemAmount;
        }

        [Serializable]
        public struct DashboardStats
        {
            public int businessesOwned;
            public int totalEmployees;
            public double dailyProfit;
            public double netWorth;
            public string topBusiness;
        }
    }
}
