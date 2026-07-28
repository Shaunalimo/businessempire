#if UNITY_EDITOR
using UnityEditor;

namespace BusinessEmpire.Editor
{
    /// <summary>
    /// Legacy menu — redirects to Build Complete UI.
    /// </summary>
    public static class SceneSetupEditor
    {
        [MenuItem("Business Empire/Setup Game Scenes")]
        public static void SetupScenes()
        {
            GameUIBuilder.BuildAll();
        }
    }
}
#endif
