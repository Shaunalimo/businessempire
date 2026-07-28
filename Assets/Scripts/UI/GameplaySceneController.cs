using BusinessEmpire.Core;
using BusinessEmpire.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Wires gameplay dashboard action buttons and ensures the dashboard is visible on play.
    /// </summary>
    [DefaultExecutionOrder(100)]
    public class GameplaySceneController : MonoBehaviour
    {
        [SerializeField] private Button buyBusinessButton;
        [SerializeField] private Button upgradeBusinessButton;
        [SerializeField] private Button earnMoneyButton;

        private void Start()
        {
            GameBootstrap.EnsureManagersExist();
            UIGameDataBridge.UseDummyData = false;

            buyBusinessButton?.onClick.AddListener(() =>
            {
                AudioManager.Instance?.PlayButtonClick();
                UINavigationManager.Instance?.ShowScreen(UIScreenId.BuyBusiness);
            });

            upgradeBusinessButton?.onClick.AddListener(() =>
            {
                AudioManager.Instance?.PlayButtonClick();
                UINavigationManager.Instance?.ShowScreen(UIScreenId.UpgradeBusiness);
            });

            earnMoneyButton?.onClick.AddListener(() =>
            {
                AudioManager.Instance?.PlayButtonClick();
                EconomyManager.Instance?.AddCash(1000d);
            });

            UINavigationManager.Instance?.ShowScreen(UIScreenId.Dashboard, addToHistory: false);
        }
    }
}
