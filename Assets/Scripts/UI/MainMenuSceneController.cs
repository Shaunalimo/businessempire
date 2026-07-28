using BusinessEmpire.Core;
using BusinessEmpire.Managers;
using BusinessEmpire.Monetization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Wires the saved MainMenu scene UI (New Game, Continue, Settings, Exit).
    /// </summary>
    public class MainMenuSceneController : MonoBehaviour
    {
        private const string GameplayScene = "Gameplay";

        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button settingsCloseButton;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private TextMeshProUGUI versionText;

        private void Start()
        {
            GameBootstrap.EnsureManagersExist();
            MonetizationManager.Instance?.EnterMainMenu();
            AudioManager.Instance?.PlayMainMenuMusic();

            if (versionText != null)
                versionText.text = "v0.1 Prototype";

            if (continueButton != null)
                continueButton.interactable = SaveManager.Instance.HasSave;

            newGameButton?.onClick.AddListener(StartNewGame);
            continueButton?.onClick.AddListener(ContinueGame);
            settingsButton?.onClick.AddListener(ShowSettings);
            settingsCloseButton?.onClick.AddListener(HideSettings);
            exitButton?.onClick.AddListener(ExitGame);
        }

        private void HideSettings()
        {
            AudioManager.Instance?.PlayButtonClick();
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }

        private static void StartNewGame()
        {
            AudioManager.Instance?.PlayButtonClick();
            SaveManager.Instance.DeleteSave();
            UIGameDataBridge.UseDummyData = false;
            SceneManager.LoadScene(GameplayScene);
        }

        private static void ContinueGame()
        {
            AudioManager.Instance?.PlayButtonClick();
            UIGameDataBridge.UseDummyData = false;
            SceneManager.LoadScene(GameplayScene);
        }

        private void ShowSettings()
        {
            AudioManager.Instance?.PlayButtonClick();
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
        }

        private static void ExitGame()
        {
            AudioManager.Instance?.PlayButtonClick();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
