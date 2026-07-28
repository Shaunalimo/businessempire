using System.Collections.Generic;
using System.Linq;
using BusinessEmpire.Core;
using BusinessEmpire.Data;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Registry and runtime logic for all business types and owned instances.
    /// </summary>
    public class BusinessManager : SingletonMonoBehaviour<BusinessManager>
    {
        [Header("Business Definitions")]
        [SerializeField] private List<BusinessDefinition> businessDefinitions = new List<BusinessDefinition>();

        private Dictionary<BusinessType, BusinessDefinition> _definitionMap = new Dictionary<BusinessType, BusinessDefinition>();

        protected override void OnSingletonAwake()
        {
            if (businessDefinitions == null || businessDefinitions.Count == 0)
            {
                var loaded = Resources.LoadAll<BusinessDefinition>("BusinessDefinitions");
                if (loaded != null && loaded.Length > 0)
                    businessDefinitions = loaded.OrderBy(d => (int)d.type).ToList();
                else
                    businessDefinitions = DefaultBusinessFactory.CreateDefaults();
            }
            RebuildDefinitionMap();
        }

        public void RebuildDefinitionMap()
        {
            _definitionMap.Clear();
            foreach (var def in businessDefinitions)
            {
                if (def != null && !_definitionMap.ContainsKey(def.type))
                    _definitionMap[def.type] = def;
            }
        }

        public BusinessDefinition GetDefinition(BusinessType type)
        {
            _definitionMap.TryGetValue(type, out var def);
            if (def != null) BusinessIconLoader.ApplyIconIfMissing(def);
            return def;
        }

        public BusinessInstanceData GetInstance(BusinessType type)
        {
            return SaveManager.Instance.CurrentSave.businesses
                .FirstOrDefault(b => b.type == type);
        }

        public IEnumerable<BusinessInstanceData> GetOwnedBusinesses()
        {
            return SaveManager.Instance.CurrentSave.businesses.Where(b => b.isOwned);
        }

        /// <summary>Attempt to purchase a business.</summary>
        public bool TryPurchaseBusiness(BusinessType type)
        {
            var def = GetDefinition(type);
            var instance = GetInstance(type);
            if (def == null || instance == null || instance.isOwned) return false;

            if (!CanPurchase(type)) return false;
            if (!EconomyManager.Instance.TrySpendCash(def.purchaseCost)) return false;

            instance.isOwned = true;
            instance.purchasedTimestamp = Utils.TimeHelper.UnixNow;
            instance.employeeCount = def.startingEmployeeSlots;

            var save = SaveManager.Instance.CurrentSave;
            save.businessesOwned++;

            GameEvents.RaiseBusinessPurchased();
            GameEvents.RaiseCashChanged(save.cash);
            return true;
        }

        public bool CanPurchase(BusinessType type)
        {
            var def = GetDefinition(type);
            var instance = GetInstance(type);
            if (def == null || instance == null || instance.isOwned) return false;

            var save = SaveManager.Instance.CurrentSave;
            if (save.cash < def.purchaseCost) return false;
            if (save.playerLevel < def.requiredPlayerLevel) return false;

            if (def.requiresPrerequisite)
            {
                var prereq = GetInstance(def.prerequisiteBusiness);
                if (prereq == null || !prereq.isOwned) return false;
            }

            return true;
        }

        /// <summary>Upgrade business level.</summary>
        public bool TryUpgradeBusiness(BusinessType type)
        {
            var def = GetDefinition(type);
            var instance = GetInstance(type);
            if (def == null || instance == null || !instance.isOwned) return false;
            if (instance.level >= GameConstants.MaxBusinessLevel) return false;

            double cost = def.GetUpgradeCost(instance.level);
            if (!EconomyManager.Instance.TrySpendCash(cost)) return false;

            instance.level++;
            GameEvents.RaiseBusinessUpgraded();
            GameEvents.RaiseBusinessUpgraded(instance.level);
            return true;
        }

        /// <summary>Calculate and apply daily income/expenses for all owned businesses.</summary>
        public double ProcessDailyCycle()
        {
            double totalProfit = 0d;
            var save = SaveManager.Instance.CurrentSave;

            foreach (var instance in GetOwnedBusinesses())
            {
                if (!instance.isActive) continue;

                var def = GetDefinition(instance.type);
                if (def == null) continue;

                double marketingBonus = MarketingManager.Instance.GetBusinessBonus(instance.type);
                double eventMultiplier = EventManager.Instance.GetBusinessMultiplier(instance.type);
                double rdBonus = ResearchManager.Instance.GlobalIncomeMultiplier;

                double income = def.GetDailyIncome(instance.level, instance.employeeCount, marketingBonus, eventMultiplier) * rdBonus;
                double salaries = def.GetDailySalaries(instance.employeeCount);
                double profit = income - salaries;

                instance.lifetimeRevenue += income;
                instance.lifetimeExpenses += salaries;
                instance.lastDayProfit = profit;

                EconomyManager.Instance.AddCash(profit);
                totalProfit += profit;
            }

            return totalProfit;
        }

        /// <summary>Get projected daily profit for UI display.</summary>
        public double GetProjectedDailyProfit(BusinessType type)
        {
            var def = GetDefinition(type);
            var instance = GetInstance(type);
            if (def == null || instance == null || !instance.isOwned) return 0d;

            double marketingBonus = MarketingManager.Instance.GetBusinessBonus(type);
            double eventMultiplier = EventManager.Instance.GetBusinessMultiplier(type);
            double rdBonus = ResearchManager.Instance.GlobalIncomeMultiplier;

            double income = def.GetDailyIncome(instance.level, instance.employeeCount, marketingBonus, eventMultiplier) * rdBonus;
            return income - def.GetDailySalaries(instance.employeeCount);
        }

        public List<BusinessDefinition> GetAllDefinitions() => businessDefinitions;
    }
}
