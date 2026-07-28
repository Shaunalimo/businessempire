using BusinessEmpire.Core;
using BusinessEmpire.Data;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Employee hiring and management per business.
    /// </summary>
    public class EmployeeManager : SingletonMonoBehaviour<EmployeeManager>
    {
        [SerializeField] private double hireCostMultiplier = 500d;

        public bool TryHireEmployee(BusinessType type)
        {
            var def = BusinessManager.Instance.GetDefinition(type);
            var instance = BusinessManager.Instance.GetInstance(type);
            if (def == null || instance == null || !instance.isOwned) return false;
            if (instance.employeeCount >= def.maxEmployees) return false;
            if (instance.employeeCount >= GameConstants.MaxEmployeesPerBusiness) return false;

            double hireCost = hireCostMultiplier * (instance.employeeCount + 1);
            if (!EconomyManager.Instance.TrySpendCash(hireCost)) return false;

            instance.employeeCount++;
            SaveManager.Instance.CurrentSave.totalEmployeesHired++;
            return true;
        }

        public bool TryFireEmployee(BusinessType type)
        {
            var instance = BusinessManager.Instance.GetInstance(type);
            if (instance == null || !instance.isOwned) return false;
            if (instance.employeeCount <= 0) return false;

            instance.employeeCount--;
            return true;
        }

        public double GetHireCost(BusinessType type)
        {
            var instance = BusinessManager.Instance.GetInstance(type);
            if (instance == null) return 0d;
            return hireCostMultiplier * (instance.employeeCount + 1);
        }
    }
}
