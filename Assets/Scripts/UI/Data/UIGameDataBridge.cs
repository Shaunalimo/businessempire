using System.Collections.Generic;
using BusinessEmpire.Data;
using BusinessEmpire.Managers;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Bridges UI view models to live game data when available, otherwise uses dummy preview data.
    /// </summary>
    public static class UIGameDataBridge
    {
        /// <summary>When true, all screens use UIDummyDataProvider (prototype/preview mode).</summary>
        public static bool UseDummyData { get; set; } = true;

        public static bool HasLiveSave =>
            !UseDummyData && SaveManager.Instance?.CurrentSave != null;

        public static UIViewModels.PlayerHeader GetPlayerHeader()
        {
            if (!HasLiveSave) return UIDummyDataProvider.GetPlayerHeader();

            var save = SaveManager.Instance.CurrentSave;
            return new UIViewModels.PlayerHeader
            {
                playerName = save.playerName,
                cash = save.cash,
                gems = save.premiumCurrency,
                companyLevel = save.playerLevel,
                reputation = save.reputation
            };
        }

        public static UIViewModels.DashboardStats GetDashboardStats()
        {
            if (!HasLiveSave) return UIDummyDataProvider.GetDashboardStats();

            var save = SaveManager.Instance.CurrentSave;
            double dailyProfit = 0;
            string top = "—";
            double topProfit = 0;

            foreach (var b in BusinessManager.Instance.GetOwnedBusinesses())
            {
                dailyProfit += b.lastDayProfit;
                if (b.lastDayProfit > topProfit)
                {
                    topProfit = b.lastDayProfit;
                    var def = BusinessManager.Instance.GetDefinition(b.type);
                    top = def?.displayName ?? b.type.ToString();
                }
            }

            return new UIViewModels.DashboardStats
            {
                businessesOwned = save.businessesOwned,
                totalEmployees = save.totalEmployeesHired,
                dailyProfit = dailyProfit,
                netWorth = save.cash,
                topBusiness = top
            };
        }

        public static List<UIViewModels.BusinessCard> GetBusinesses()
        {
            if (!HasLiveSave) return UIDummyDataProvider.GetBusinesses();

            var list = new List<UIViewModels.BusinessCard>();
            foreach (var def in BusinessManager.Instance.GetAllDefinitions())
            {
                var inst = BusinessManager.Instance.GetInstance(def.type);
                if (inst == null) continue;
                list.Add(ToBusinessCard(def, inst));
            }
            return list;
        }

        public static UIViewModels.BusinessCard GetBusinessDetail(BusinessType type)
        {
            if (!HasLiveSave) return UIDummyDataProvider.GetBusinessDetail(type);

            var def = BusinessManager.Instance.GetDefinition(type);
            var inst = BusinessManager.Instance.GetInstance(type);
            if (def == null || inst == null)
                return UIDummyDataProvider.GetBusinessDetail(type);
            return ToBusinessCard(def, inst);
        }

        public static List<UIViewModels.EmployeeSlot> GetEmployeeSlots()
        {
            if (!HasLiveSave) return UIDummyDataProvider.GetEmployeeSlots();

            var slots = new List<UIViewModels.EmployeeSlot>();
            foreach (var inst in BusinessManager.Instance.GetOwnedBusinesses())
            {
                var def = BusinessManager.Instance.GetDefinition(inst.type);
                if (def == null) continue;
                slots.Add(new UIViewModels.EmployeeSlot
                {
                    businessType = inst.type,
                    businessName = def.displayName,
                    current = inst.employeeCount,
                    max = def.maxEmployees,
                    hireCost = EmployeeManager.Instance.GetHireCost(inst.type),
                    salaryPerDay = def.GetDailySalaries(inst.employeeCount)
                });
            }
            return slots;
        }

        public static List<UIViewModels.ShopProduct> GetShopProducts()
        {
            if (!HasLiveSave) return UIDummyDataProvider.GetShopProducts();

            return UIDummyDataProvider.GetShopProducts();
        }

        private static UIViewModels.BusinessCard ToBusinessCard(BusinessDefinition def, BusinessInstanceData inst)
        {
            double profit = BusinessManager.Instance.GetProjectedDailyProfit(def.type);
            double income = def.GetDailyIncome(inst.level, inst.employeeCount, 1d, 1d);
            return new UIViewModels.BusinessCard
            {
                type = def.type,
                name = def.displayName,
                description = def.description,
                isOwned = inst.isOwned,
                level = inst.level,
                employees = inst.employeeCount,
                purchaseCost = def.purchaseCost,
                dailyProfit = profit,
                dailyIncome = income,
                iconKey = def.type.ToString()
            };
        }
    }
}
