using System.Collections;
using BusinessEmpire.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 1: Splash — brand intro, auto-advances to main menu.
    /// </summary>
    public class SplashScreen : UIScreenBase
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI taglineText;
        [SerializeField] private Image logoPlaceholder;
        [SerializeField] private float autoAdvanceSeconds = 2.5f;
        [SerializeField] private Button tapToContinue;

        private void Awake()
        {
            screenId = UIScreenId.Splash;
            hideTopBar = true;
            hideBottomNav = true;
            tapToContinue?.onClick.AddListener(GoToMainMenu);
        }

        public override void OnScreenShown()
        {
            base.OnScreenShown();
            StopAllCoroutines();
            StartCoroutine(AutoAdvance());
        }

        public override void Refresh()
        {
            if (titleText != null) titleText.text = GameConstants.GameTitle;
            if (taglineText != null) taglineText.text = "Build Your Empire";
            if (logoPlaceholder != null) logoPlaceholder.color = UITheme.AccentPrimary;
        }

        private IEnumerator AutoAdvance()
        {
            yield return new WaitForSeconds(autoAdvanceSeconds);
            GoToMainMenu();
        }

        private void GoToMainMenu() => NavigateTo(UIScreenId.MainMenu);
    }
}
