using UnityEngine;
using UnityEngine.SceneManagement;

namespace BusinessEmpire.Core
{
    /// <summary>
    /// Loads the main menu after bootstrap initialization.
    /// Attach to Bootstrap scene alongside GameBootstrap.
    /// </summary>
    public class AppInitializer : MonoBehaviour
    {
        [SerializeField] private string firstSceneName = "MainMenu";
        [SerializeField] private bool loadFirstSceneOnStart = true;

        private void Start()
        {
            GameBootstrap.EnsureManagersExist();

            if (loadFirstSceneOnStart && !string.IsNullOrEmpty(firstSceneName))
            {
                var active = SceneManager.GetActiveScene().name;
                if (active != firstSceneName)
                    SceneManager.LoadScene(firstSceneName);
            }
        }
    }
}
