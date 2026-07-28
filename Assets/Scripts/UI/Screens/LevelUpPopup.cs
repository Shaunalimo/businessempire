using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 19: Success / Level Up popup overlay.
    /// </summary>
    public class LevelUpPopup : UIPopupBase
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button okButton;

        private void Awake()
        {
            screenId = UIScreenId.LevelUpPopup;
            hideTopBar = true;
            hideBottomNav = true;
            okButton?.onClick.AddListener(() => Close());
        }

        public override void Refresh()
        {
            if (UIScreenContext.PendingLevelUp > 0)
            {
                if (titleText != null) titleText.text = "Level Up!";
                if (messageText != null)
                    messageText.text = $"Business reached Level {UIScreenContext.PendingLevelUp}!\n+15% income boost unlocked.";
            }
            else
            {
                if (titleText != null) titleText.text = "Success!";
                if (messageText != null) messageText.text = "Business acquired!\nYour empire grows stronger.";
            }
        }
    }
}
