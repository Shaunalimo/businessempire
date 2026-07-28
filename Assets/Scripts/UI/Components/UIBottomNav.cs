using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Bottom tab navigation: Dashboard, Businesses, Employees, Reports, Shop.
    /// </summary>
    public class UIBottomNav : MonoBehaviour
    {
        [Serializable]
        public struct NavTab
        {
            public UIScreenId screenId;
            public Button button;
            public Image iconPlaceholder;
            public TextMeshProUGUI label;
            public Color activeColor;
        }

        [SerializeField] private NavTab[] tabs;

        public event Action<UIScreenId> OnTabSelected;

        private void Awake()
        {
            foreach (var tab in tabs)
            {
                var captured = tab;
                tab.button?.onClick.AddListener(() => OnTabSelected?.Invoke(captured.screenId));
            }
        }

        public void Setup(NavTab[] navTabs)
        {
            tabs = navTabs;
            foreach (var tab in tabs)
            {
                var captured = tab;
                tab.button?.onClick.AddListener(() => OnTabSelected?.Invoke(captured.screenId));
            }
        }

        public void SetActiveTab(UIScreenId id)
        {
            foreach (var tab in tabs)
            {
                bool active = tab.screenId == id;
                if (tab.label != null)
                    tab.label.color = active ? UITheme.TextAccent : UITheme.TextSecondary;
                if (tab.iconPlaceholder != null)
                    tab.iconPlaceholder.color = active ? tab.activeColor : UITheme.BackgroundElevated;
            }
        }
    }
}
