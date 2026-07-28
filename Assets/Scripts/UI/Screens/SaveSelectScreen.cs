using BusinessEmpire.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 3: New Game / Continue — save slot selection.
    /// </summary>
    public class SaveSelectScreen : UIScreenBase
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI continueInfoText;

        private void Awake()
        {
            screenId = UIScreenId.SaveSelect;
            hideTopBar = true;
            hideBottomNav = true;
            newGameButton?.onClick.AddListener(OnNewGame);
            continueButton?.onClick.AddListener(OnContinue);
            backButton?.onClick.AddListener(() => NavigateTo(UIScreenId.MainMenu));
        }

        public override void Refresh()
        {
            var header = UIGameDataBridge.GetPlayerHeader();
            if (continueInfoText != null)
                continueInfoText.text = $"Continue as {header.playerName} · Lv.{header.companyLevel} · {header.cash:N0} cash";
        }

        private void OnNewGame()
        {
            SaveManager.Instance.DeleteSave();
            UIGameDataBridge.UseDummyData = false;
            NavigateTo(UIScreenId.Dashboard);
        }

        private void OnContinue()
        {
            UIGameDataBridge.UseDummyData = false;
            NavigateTo(UIScreenId.Dashboard);
        }
    }
}
