using BusinessEmpire.Core;
using BusinessEmpire.Monetization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 2: Main Menu — entry point with play and settings.
    /// </summary>
    public class MainMenuScreen : UIScreenBase
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Image heroPlaceholder;

        private void Awake()
        {
            screenId = UIScreenId.MainMenu;
            hideTopBar = true;
            hideBottomNav = true;
            playButton?.onClick.AddListener(() => NavigateTo(UIScreenId.SaveSelect));
            settingsButton?.onClick.AddListener(() => NavigateTo(UIScreenId.Settings));
            shopButton?.onClick.AddListener(() => NavigateTo(UIScreenId.Shop));
        }

        public override void OnScreenShown()
        {
            base.OnScreenShown();
            MonetizationManager.Instance?.EnterMainMenu();
        }

        public override void OnScreenHidden()
        {
            MonetizationManager.Instance?.ExitMainMenu();
            base.OnScreenHidden();
        }

        public override void Refresh()
        {
            if (titleText != null) titleText.text = GameConstants.GameTitle;
            if (heroPlaceholder != null) heroPlaceholder.color = UITheme.AccentSecondary;
        }
    }
}
