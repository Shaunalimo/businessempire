using BusinessEmpire.Data;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Loads business icon sprites from Resources when ScriptableObject icons are missing.
    /// </summary>
    public static class BusinessIconLoader
    {
        private const string IconResourcePath = "BusinessIcons";

        public static Sprite GetIcon(BusinessType type)
        {
            return Resources.Load<Sprite>($"{IconResourcePath}/{type}");
        }

        public static void ApplyIconIfMissing(BusinessDefinition definition)
        {
            if (definition == null || definition.icon != null) return;
            definition.icon = GetIcon(definition.type);
        }
    }
}
