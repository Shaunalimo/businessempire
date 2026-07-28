using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 18: Game Over / Bankruptcy — failure state with restart.
    /// </summary>
    public class GameOverScreen : UIScreenBase
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private void Awake()
        {
            screenId = UIScreenId.GameOver;
            hideTopBar = true;
            hideBottomNav = true;
            restartButton?.onClick.AddListener(() => NavigateTo(UIScreenId.SaveSelect));
            mainMenuButton?.onClick.AddListener(() => NavigateTo(UIScreenId.MainMenu));
        }

        public override void Refresh()
        {
            if (titleText != null) titleText.text = "Bankruptcy";
            if (messageText != null)
                messageText.text = "Your empire has collapsed under debt.\n\nLearn from this setback and build again stronger.";
        }
    }
}
