using System;

namespace BusinessEmpire.Data
{
    /// <summary>
    /// Runtime save data for a single owned business.
    /// </summary>
    [Serializable]
    public class BusinessInstanceData
    {
        public BusinessType type;
        public int level = 1;
        public int employeeCount;
        public bool isOwned;
        public bool isActive = true;
        public double lifetimeRevenue;
        public double lifetimeExpenses;
        public double lastDayProfit;
        public double customerSatisfaction = 0.75d;
        public long purchasedTimestamp;

        public BusinessInstanceData() { }

        public BusinessInstanceData(BusinessType businessType)
        {
            type = businessType;
        }

        public double NetProfit => lifetimeRevenue - lifetimeExpenses;
    }
}
