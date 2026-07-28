using System.Collections.Generic;
using BusinessEmpire.Data;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Provides default business definitions at runtime when ScriptableObjects are not assigned.
    /// </summary>
    public static class DefaultBusinessFactory
    {
        public static List<BusinessDefinition> CreateDefaults()
        {
            return new List<BusinessDefinition>
            {
                Create(BusinessType.GroceryShop, "Grocery Shop", "Essential retail with steady foot traffic.",
                    8000, 420, 80, 1, 0, false),
                Create(BusinessType.CoffeeShop, "Coffee Shop", "High-margin beverages and loyal morning customers.",
                    12000, 580, 90, 1, 0, false),
                Create(BusinessType.Restaurant, "Restaurant", "Full-service dining with premium ticket sizes.",
                    25000, 950, 120, 2, 1, true, BusinessType.GroceryShop),
                Create(BusinessType.ClothingStore, "Clothing Store", "Fashion retail with seasonal demand spikes.",
                    35000, 1100, 110, 2, 1, true, BusinessType.CoffeeShop),
                Create(BusinessType.SoftwareCompany, "Software Company", "Scalable digital products and subscriptions.",
                    75000, 2200, 200, 3, 2, true, BusinessType.Restaurant),
                Create(BusinessType.LogisticsCompany, "Logistics Company", "Delivery fleet serving regional clients.",
                    120000, 3200, 180, 3, 2, true, BusinessType.ClothingStore),
                Create(BusinessType.Factory, "Factory", "Manufacturing at scale with high overhead.",
                    200000, 4800, 250, 4, 3, true, BusinessType.LogisticsCompany),
                Create(BusinessType.RealEstateAgency, "Real Estate Agency", "Property commissions and portfolio growth.",
                    350000, 7500, 300, 5, 3, true, BusinessType.Factory)
            };
        }

        private static BusinessDefinition Create(
            BusinessType type, string name, string desc,
            double cost, double income, double salary,
            int playerLevel, int maxEmployees, bool requiresPrereq,
            BusinessType prereq = BusinessType.GroceryShop)
        {
            var def = ScriptableObject.CreateInstance<BusinessDefinition>();
            def.type = type;
            def.displayName = name;
            def.description = desc;
            def.purchaseCost = cost;
            def.baseDailyIncome = income;
            def.baseSalaryPerEmployee = salary;
            def.requiredPlayerLevel = playerLevel;
            def.maxEmployees = maxEmployees;
            def.startingEmployeeSlots = 2;
            def.requiresPrerequisite = requiresPrereq;
            def.prerequisiteBusiness = prereq;
            return def;
        }
    }
}
