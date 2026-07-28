using UnityEngine;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Base for overlay popups (level up, offline earnings, etc.).
    /// </summary>
    public abstract class UIPopupBase : UIScreenBase
    {
        [SerializeField] private UIAnimator animator;

        public override void OnScreenShown()
        {
            gameObject.SetActive(true);
            animator?.Show();
            Refresh();
        }

        public override void OnScreenHidden()
        {
            if (animator != null)
                animator.Hide(() => gameObject.SetActive(false));
            else
                gameObject.SetActive(false);
        }

        public void Close() => UINavigationManager.Instance?.ClosePopup(screenId);
    }
}
