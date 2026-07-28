using UnityEngine;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Enables dummy data mode for the UI mobile prototype scene.
    /// </summary>
    public class UIPrototypeBootstrap : MonoBehaviour
    {
        [SerializeField] private bool useDummyData = true;
        [SerializeField] private bool showOfflinePopupOnStart = true;

        private void Awake()
        {
            UIGameDataBridge.UseDummyData = useDummyData;
        }

        private void Start()
        {
            if (showOfflinePopupOnStart)
            {
                Invoke(nameof(ShowOffline), 3f);
            }
        }

        private void ShowOffline()
        {
            if (UINavigationManager.Instance != null &&
                UINavigationManager.Instance.CurrentScreen == UIScreenId.Dashboard)
            {
                UINavigationManager.Instance.ShowPopup(UIScreenId.OfflineEarningsPopup);
            }
        }
    }
}
