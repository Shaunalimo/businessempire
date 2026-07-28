using UnityEngine;
using UnityEngine.SceneManagement;

namespace BusinessEmpire.Core
{
    /// <summary>
    /// Loads the first gameplay scene from Bootstrap.unity.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private string firstSceneName = "MainMenu";
        [SerializeField] private bool loadOnStart = true;

        private void Start()
        {
            if (!loadOnStart || string.IsNullOrEmpty(firstSceneName))
                return;

            var active = SceneManager.GetActiveScene().name;
            if (active != firstSceneName)
                SceneManager.LoadScene(firstSceneName);
        }
    }
}
