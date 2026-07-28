using System.Collections.Generic;
using BusinessEmpire.Data;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Sample dummy data so all UI screens can be previewed without a live save game.
    /// </summary>
    public static class UIDummyDataProvider
    {
        public static UIViewModels.PlayerHeader GetPlayerHeader() => new UIViewModels.PlayerHeader
        {
            playerName = "Alex Chen",
            cash = 48750,
            gems = 120,
            companyLevel = 7,
            reputation = 84
        };

        public static UIViewModels.DashboardStats GetDashboardStats() => new UIViewModels.DashboardStats
        {
            businessesOwned = 4,
            totalEmployees = 28,
            dailyProfit = 8420,
            netWorth = 312000,
            topBusiness = "Software Company"
        };

        public static List<UIViewModels.BusinessCard> GetBusinesses()
        {
            return new List<UIViewModels.BusinessCard>
            {
                Card(BusinessType.GroceryShop, "Grocery Shop", true, 3, 8, 8000, 420, 680),
                Card(BusinessType.CoffeeShop, "Coffee Shop", true, 2, 5, 12000, 580, 920),
                Card(BusinessType.Restaurant, "Restaurant", true, 4, 12, 25000, 950, 2100),
                Card(BusinessType.ClothingStore, "Clothing Store", true, 2, 6, 35000, 1100, 1450),
                Card(BusinessType.SoftwareCompany, "Software Company", false, 0, 0, 75000, 2200, 0),
                Card(BusinessType.LogisticsCompany, "Logistics Co.", false, 0, 0, 120000, 3200, 0),
                Card(BusinessType.Factory, "Factory", false, 0, 0, 200000, 4800, 0),
                Card(BusinessType.RealEstateAgency, "Real Estate", false, 0, 0, 350000, 7500, 0)
            };
        }

        public static UIViewModels.BusinessCard GetBusinessDetail(BusinessType type)
        {
            foreach (var b in GetBusinesses())
                if (b.type == type) return b;
            return GetBusinesses()[0];
        }

        public static List<UIViewModels.EmployeeSlot> GetEmployeeSlots()
        {
            return new List<UIViewModels.EmployeeSlot>
            {
                new UIViewModels.EmployeeSlot { businessName = "Grocery Shop", current = 8, max = 20, hireCost = 4000, salaryPerDay = 640 },
                new UIViewModels.EmployeeSlot { businessName = "Coffee Shop", current = 5, max = 20, hireCost = 3000, salaryPerDay = 450 },
                new UIViewModels.EmployeeSlot { businessName = "Restaurant", current = 12, max = 25, hireCost = 6000, salaryPerDay = 1440 },
                new UIViewModels.EmployeeSlot { businessName = "Clothing Store", current = 6, max = 20, hireCost = 3500, salaryPerDay = 660 }
            };
        }

        public static List<UIViewModels.MarketingOption> GetMarketingOptions()
        {
            return new List<UIViewModels.MarketingOption>
            {
                new UIViewModels.MarketingOption { id = "social", name = "Social Media Blitz", description = "Viral posts boost foot traffic.", cost = 2500, bonusText = "+15% income", durationDays = 3 },
                new UIViewModels.MarketingOption { id = "tv", name = "TV Commercial", description = "Prime-time brand exposure.", cost = 15000, bonusText = "+30% income", durationDays = 7 },
                new UIViewModels.MarketingOption { id = "influencer", name = "Influencer Deal", description = "Celebrity endorsement campaign.", cost = 50000, bonusText = "+50% income", durationDays = 14 }
            };
        }

        public static List<UIViewModels.LoanOption> GetLoans()
        {
            return new List<UIViewModels.LoanOption>
            {
                new UIViewModels.LoanOption { id = "small", name = "Starter Loan", amount = 10000, terms = "30 days · 0.2% daily interest" },
                new UIViewModels.LoanOption { id = "medium", name = "Growth Loan", amount = 50000, terms = "60 days · 0.15% daily interest" },
                new UIViewModels.LoanOption { id = "large", name = "Enterprise Loan", amount = 250000, terms = "90 days · 0.1% daily interest" }
            };
        }

        public static List<UIViewModels.InvestmentOption> GetInvestments()
        {
            return new List<UIViewModels.InvestmentOption>
            {
                new UIViewModels.InvestmentOption { id = "bond", name = "Savings Bond", minAmount = 5000, returnText = "+5% return", days = 7 },
                new UIViewModels.InvestmentOption { id = "stock", name = "Stock Portfolio", minAmount = 25000, returnText = "+12% return", days = 14 },
                new UIViewModels.InvestmentOption { id = "vc", name = "Venture Capital", minAmount = 100000, returnText = "+25% return", days = 30 }
            };
        }

        public static UIViewModels.ReportSummary GetReport()
        {
            return new UIViewModels.ReportSummary
            {
                totalRevenue = 142800,
                totalExpenses = 68400,
                netProfit = 74400,
                daysPlayed = 42,
                lines = new List<UIViewModels.BusinessProfitLine>
                {
                    new UIViewModels.BusinessProfitLine { name = "Restaurant", profit = 3200 },
                    new UIViewModels.BusinessProfitLine { name = "Software Co.", profit = 2800 },
                    new UIViewModels.BusinessProfitLine { name = "Clothing Store", profit = 1450 },
                    new UIViewModels.BusinessProfitLine { name = "Coffee Shop", profit = 920 }
                }
            };
        }

        public static List<UIViewModels.DailyRewardDay> GetDailyRewards()
        {
            return new List<UIViewModels.DailyRewardDay>
            {
                new UIViewModels.DailyRewardDay { day = 1, cash = 1000, gems = 1, isClaimed = true },
                new UIViewModels.DailyRewardDay { day = 2, cash = 2000, gems = 2, isClaimed = true },
                new UIViewModels.DailyRewardDay { day = 3, cash = 3500, gems = 3, isToday = true },
                new UIViewModels.DailyRewardDay { day = 4, cash = 5000, gems = 5 },
                new UIViewModels.DailyRewardDay { day = 5, cash = 7500, gems = 7 },
                new UIViewModels.DailyRewardDay { day = 6, cash = 10000, gems = 10 },
                new UIViewModels.DailyRewardDay { day = 7, cash = 25000, gems = 25 }
            };
        }

        public static List<UIViewModels.AchievementItem> GetAchievements()
        {
            return new List<UIViewModels.AchievementItem>
            {
                new UIViewModels.AchievementItem { id = "first", title = "Entrepreneur", description = "Own your first business", unlocked = true, gemReward = 5 },
                new UIViewModels.AchievementItem { id = "million", title = "Millionaire", description = "Reach $1,000,000 cash", unlocked = false, gemReward = 25 },
                new UIViewModels.AchievementItem { id = "empire", title = "Business Empire", description = "Own all 8 businesses", unlocked = false, gemReward = 100 },
                new UIViewModels.AchievementItem { id = "workforce", title = "Workforce Leader", description = "Hire 50 employees", unlocked = true, gemReward = 20 },
                new UIViewModels.AchievementItem { id = "veteran", title = "Veteran CEO", description = "Play 30 days", unlocked = false, gemReward = 15 }
            };
        }

        public static List<UIViewModels.ShopProduct> GetShopProducts()
        {
            return new List<UIViewModels.ShopProduct>
            {
                new UIViewModels.ShopProduct { id = "removeads", name = "Remove Ads", description = "No more interruptions", price = "$4.99", gemAmount = 0 },
                new UIViewModels.ShopProduct { id = "gems100", name = "100 Gems", description = "Premium currency pack", price = "$0.99", gemAmount = 100 },
                new UIViewModels.ShopProduct { id = "gems500", name = "500 Gems", description = "Best value pack", price = "$3.99", gemAmount = 500 },
                new UIViewModels.ShopProduct { id = "gems1200", name = "1200 Gems", description = "Mega CEO bundle", price = "$7.99", gemAmount = 1200 }
            };
        }

        public static double GetOfflineEarnings() => 12450;

        private static UIViewModels.BusinessCard Card(BusinessType t, string name, bool owned, int lvl, int emp,
            double cost, double baseIncome, double profit)
        {
            return new UIViewModels.BusinessCard
            {
                type = t, name = name, isOwned = owned, level = lvl, employees = emp,
                purchaseCost = cost, dailyIncome = baseIncome, dailyProfit = profit,
                description = $"Manage your {name.ToLower()} operations.",
                iconKey = t.ToString()
            };
        }
    }
}
