using BusinessEmpire.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Plays scene-appropriate background music on load.
    /// </summary>
    public class SceneMusicController : MonoBehaviour
    {
        [SerializeField] private bool playMainMenuMusic;
        [SerializeField] private bool playGameplayMusic;

        private void Start()
        {
            if (AudioManager.Instance == null) return;

            if (playMainMenuMusic)
                AudioManager.Instance.PlayMainMenuMusic();
            else if (playGameplayMusic)
                AudioManager.Instance.PlayGameplayMusic();
        }

        public static void AttachToScene(string sceneName)
        {
            // Called from editor setup; runtime uses serialized flags on scene objects.
        }
    }
}
