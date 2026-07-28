using System;
using UnityEngine;

namespace BusinessEmpire.Data
{
    /// <summary>
    /// ScriptableObject template defining base stats for a business type.
    /// Create instances via Assets > Create > Business Empire > Business Definition.
    /// </summary>
    [CreateAssetMenu(fileName = "BusinessDefinition", menuName = "Business Empire/Business Definition")]
    public class BusinessDefinition : ScriptableObject
    {
        [Header("Identity")]
        public BusinessType type;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Economy")]
        public double purchaseCost = 10000d;
        public double baseDailyIncome = 500d;
        public double baseSalaryPerEmployee = 100d;
        public double upgradeCostMultiplier = 1.5d;
        public double incomeGrowthPerLevel = 0.15d;
        public double customerGrowthRate = 0.05d;

        [Header("Employees")]
        public int startingEmployeeSlots = 2;
        public int maxEmployees = 20;

        [Header("Unlock")]
        public int requiredPlayerLevel = 1;
        public BusinessType prerequisiteBusiness = BusinessType.GroceryShop;
        public bool requiresPrerequisite;

        /// <summary>Calculate upgrade cost for a given level.</summary>
        public double GetUpgradeCost(int currentLevel)
        {
            return Math.Max(0d, Math.Round(purchaseCost * Math.Pow(upgradeCostMultiplier, currentLevel), 2));
        }

        /// <summary>Calculate daily income at a given level with employee and marketing bonuses.</summary>
        public double GetDailyIncome(int level, int employees, double marketingBonus, double eventMultiplier)
        {
            double levelMultiplier = 1d + (level - 1) * incomeGrowthPerLevel;
            double employeeBonus = 1d + employees * 0.02d;
            double customerGrowth = 1d + customerGrowthRate * level;
            return MathfAt(baseDailyIncome * levelMultiplier * employeeBonus * customerGrowth * marketingBonus * eventMultiplier);
        }

        /// <summary>Total daily salary expense.</summary>
        public double GetDailySalaries(int employees)
        {
            return baseSalaryPerEmployee * employees;
        }

        private static double MathfAt(double value) => Math.Max(0d, Math.Round(value, 2));
    }
}
