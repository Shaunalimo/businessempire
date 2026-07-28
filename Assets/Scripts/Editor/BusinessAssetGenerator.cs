#if UNITY_EDITOR
using BusinessEmpire.Data;
using BusinessEmpire.Managers;
using UnityEditor;
using UnityEngine;

namespace BusinessEmpire.Editor
{
    /// <summary>
    /// One-click editor utility to generate business ScriptableObject assets.
    /// </summary>
    public static class BusinessAssetGenerator
    {
        private const string AssetPath = "Assets/Resources/BusinessDefinitions";

        [MenuItem("Business Empire/Generate Business Assets")]
        public static void GenerateAll()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder(AssetPath))
                AssetDatabase.CreateFolder("Assets/Resources", "BusinessDefinitions");

            foreach (var def in DefaultBusinessFactory.CreateDefaults())
            {
                string fileName = $"{def.type}.asset";
                string fullPath = $"{AssetPath}/{fileName}";

                var existing = AssetDatabase.LoadAssetAtPath<BusinessDefinition>(fullPath);
                if (existing != null)
                {
                    existing.type = def.type;
                    existing.displayName = def.displayName;
                    existing.description = def.description;
                    existing.purchaseCost = def.purchaseCost;
                    existing.baseDailyIncome = def.baseDailyIncome;
                    existing.baseSalaryPerEmployee = def.baseSalaryPerEmployee;
                    existing.requiredPlayerLevel = def.requiredPlayerLevel;
                    existing.maxEmployees = def.maxEmployees;
                    existing.requiresPrerequisite = def.requiresPrerequisite;
                    existing.prerequisiteBusiness = def.prerequisiteBusiness;
                    EditorUtility.SetDirty(existing);
                }
                else
                {
                    AssetDatabase.CreateAsset(Object.Instantiate(def), fullPath);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Business Empire] Business definition assets generated.");

            BusinessIconGenerator.GenerateAll();
        }

        [MenuItem("Business Empire/Configure Android Portrait")]
        public static void ConfigureAndroidPortrait()
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.companyName = "Tekh Games Studio";
            PlayerSettings.productName = "Business Empire CEO Simulator";
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.tekhgames.businessempire");
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            Debug.Log("[Business Empire] Android portrait settings applied.");
        }
    }
}
#endif
