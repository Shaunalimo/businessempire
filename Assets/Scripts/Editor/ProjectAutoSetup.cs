#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BusinessEmpire.Editor
{
    /// <summary>
    /// Generates scenes, prefabs, and resources on first editor launch when assets are missing.
    /// </summary>
    [InitializeOnLoad]
    public static class ProjectAutoSetup
    {
        private const string BootstrapScene = "Assets/Scenes/Bootstrap.unity";
        private const string MainMenuScene = "Assets/Scenes/MainMenu.unity";
        private const string GameplayScene = "Assets/Scenes/Gameplay.unity";
        private const string IapCatalog = "Assets/Resources/IapProductCatalog.asset";

        static ProjectAutoSetup()
        {
            EditorApplication.delayCall += TryGenerateMissingAssets;
        }

        [MenuItem("Business Empire/Generate All Project Assets")]
        public static void ForceGenerateAll()
        {
            GameUIBuilder.BuildAll();
            MonetizationSetupEditor.SetupMonetization();
            Debug.Log("[Business Empire] All project assets generated.");
        }

        private static void TryGenerateMissingAssets()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
            {
                EditorApplication.delayCall += TryGenerateMissingAssets;
                return;
            }

            if (PlayableScenePopulator.ScenesNeedPopulation())
            {
                Debug.Log("[Business Empire] Empty playable scenes detected — populating MainMenu, Gameplay, and Bootstrap...");
                PlayableScenePopulator.PopulateAll();
                return;
            }

            if (AssetsExist()) return;

            Debug.Log("[Business Empire] Missing scenes/prefabs detected — generating complete project assets...");
            GameUIBuilder.BuildAll();
            MonetizationSetupEditor.EnsureMonetizationTestPanel();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Business Empire] Project ready. Open Bootstrap.unity or MainMenu.unity and press Play.");
        }

        private static bool AssetsExist()
        {
            return File.Exists(BootstrapScene)
                   && File.Exists(MainMenuScene)
                   && File.Exists(GameplayScene)
                   && File.Exists(IapCatalog);
        }
    }
}
#endif
