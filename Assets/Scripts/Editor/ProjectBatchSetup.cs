#if UNITY_EDITOR
using UnityEditor;

namespace BusinessEmpire.Editor
{
    /// <summary>
    /// Entry point for Unity batch mode: Unity.exe -batchmode -quit -executeMethod BusinessEmpire.Editor.ProjectBatchSetup.Run
    /// </summary>
    public static class ProjectBatchSetup
    {
        public static void Run()
        {
            PlayableScenePopulator.PopulateAll();
            ProjectAutoSetup.ForceGenerateAll();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorApplication.Exit(0);
        }
    }
}
#endif
