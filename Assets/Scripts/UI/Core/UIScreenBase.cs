using UnityEngine;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Base class for full-screen UI panels. Handles show/hide lifecycle.
    /// </summary>
    public abstract class UIScreenBase : MonoBehaviour
    {
        [SerializeField] protected UIScreenId screenId;
        [SerializeField] protected bool hideTopBar;
        [SerializeField] protected bool hideBottomNav;

        public UIScreenId ScreenId
        {
            get
            {
                if (screenId != UIScreenId.None) return screenId;
                if (System.Enum.TryParse(gameObject.name, out UIScreenId parsed))
                    return parsed;
                return UIScreenId.None;
            }
        }

        public bool HideTopBar => hideTopBar;
        public bool HideBottomNav => hideBottomNav;

        /// <summary>Populate UI from UIGameDataBridge or UIDummyDataProvider.</summary>
        public abstract void Refresh();

        protected void NavigateTo(UIScreenId id) => UINavigationManager.Instance?.ShowScreen(id);
        protected void ShowPopup(UIScreenId id) => UINavigationManager.Instance?.ShowPopup(id);
        protected void GoBack() => UINavigationManager.Instance?.GoBack();

        /// <summary>Called by navigation manager when screen becomes visible.</summary>
        public virtual void OnScreenShown()
        {
            gameObject.SetActive(true);
            Refresh();
        }

        public virtual void OnScreenHidden() => gameObject.SetActive(false);
    }
}
