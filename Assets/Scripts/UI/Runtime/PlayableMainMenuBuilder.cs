using BusinessEmpire.Core;
using BusinessEmpire.Managers;
using BusinessEmpire.Monetization;
using BusinessEmpire.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Runtime
{
    /// <summary>
    /// Runtime fallback for MainMenu when the scene has no editor-built Canvas.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class PlayableMainMenuBuilder : MonoBehaviour
    {
        private const string GameplayScene = "Gameplay";

        private void Awake()
        {
            if (Object.FindObjectOfType<Canvas>() != null)
            {
                Destroy(this);
                return;
            }

            RuntimeUIFactory.EnsureEventSystem();
            GameBootstrap.EnsureManagersExist();
            MonetizationManager.Instance?.EnterMainMenu();

            var canvas = RuntimeUIFactory.CreateCanvas("MainMenuCanvas");
            RuntimeUIFactory.CreatePanel(canvas.transform, "Background", UITheme.BackgroundDark);

            RuntimeUIFactory.CreateText(canvas.transform, "Title", "AI Business Tycoon", 56, UITheme.TextPrimary)
                .rectTransform.anchoredPosition = new Vector2(0, 500);
            RuntimeUIFactory.CreateText(canvas.transform, "Subtitle", "Build your global business empire", 36,
                UITheme.AccentPrimary).rectTransform.anchoredPosition = new Vector2(0, 400);

            var newGame = RuntimeUIFactory.CreateButton(canvas.transform, "NewGameButton", "New Game", UITheme.AccentSuccess);
            newGame.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100);
            newGame.onClick.AddListener(StartNewGame);

            var cont = RuntimeUIFactory.CreateButton(canvas.transform, "ContinueButton", "Continue", UITheme.AccentPrimary);
            cont.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -40);
            cont.onClick.AddListener(ContinueGame);
            cont.interactable = SaveManager.Instance.HasSave;

            var exit = RuntimeUIFactory.CreateButton(canvas.transform, "ExitButton", "Exit", UITheme.AccentDanger, 80);
            exit.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -300);
            exit.onClick.AddListener(() => Application.Quit());

            RuntimeUIFactory.CreateText(canvas.transform, "VersionText", "v0.1 Prototype", 22, UITheme.TextMuted)
                .rectTransform.anchoredPosition = new Vector2(0, -450);

            new GameObject("UIManager").AddComponent<UIManager>();
            new GameObject("AudioManager").AddComponent<AudioManager>();
            new GameObject("SceneManagersRoot").AddComponent<SceneManagersRoot>();
        }

        private static void StartNewGame()
        {
            SaveManager.Instance.DeleteSave();
            UIGameDataBridge.UseDummyData = false;
            SceneManager.LoadScene(GameplayScene);
        }

        private static void ContinueGame()
        {
            UIGameDataBridge.UseDummyData = false;
            SceneManager.LoadScene(GameplayScene);
        }
    }
}
