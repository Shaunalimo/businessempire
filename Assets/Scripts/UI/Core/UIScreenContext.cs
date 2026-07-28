using BusinessEmpire.Data;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Shared UI navigation context (selected business, etc.) — not game save data.
    /// </summary>
    public static class UIScreenContext
    {
        public static BusinessType SelectedBusiness { get; set; } = BusinessType.GroceryShop;
        public static int PendingLevelUp { get; set; }
    }
}
