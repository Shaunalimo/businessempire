using System.Collections.Generic;
using BusinessEmpire.Core;
using UnityEngine;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Central navigation controller for the mobile UI prototype.
    /// Manages screen stack, popups, top bar, and bottom nav visibility.
    /// </summary>
    public class UINavigationManager : SingletonMonoBehaviour<UINavigationManager>
    {
        [SerializeField] private UITopBar topBar;
        [SerializeField] private UIBottomNav bottomNav;
        [SerializeField] private Transform screenContainer;
        [SerializeField] private Transform popupContainer;
        [SerializeField] private bool startWithSplash = true;

        private readonly Dictionary<UIScreenId, UIScreenBase> _screens = new Dictionary<UIScreenId, UIScreenBase>();
        private readonly Stack<UIScreenId> _history = new Stack<UIScreenId>();
        private UIScreenId _current = UIScreenId.None;
        private UIScreenBase _activePopup;

        public UIScreenId CurrentScreen => _current;

        protected override void OnSingletonAwake() { }

        public void Setup(UITopBar top, UIBottomNav bottom, Transform screens, Transform popups, bool splash = false)
        {
            topBar = top;
            bottomNav = bottom;
            screenContainer = screens;
            popupContainer = popups;
            startWithSplash = splash;
        }

        private void Start()
        {
            RegisterAllScreens();

            if (bottomNav != null)
                bottomNav.OnTabSelected += HandleTabSelected;

            if (startWithSplash)
                ShowScreen(UIScreenId.Splash, addToHistory: false);
            else
                ShowScreen(UIScreenId.Dashboard, addToHistory: false);
        }

        /// <summary>Register screen components found under screen/popup containers.</summary>
        public void RegisterAllScreens()
        {
            _screens.Clear();
            if (screenContainer != null)
                RegisterFromTransform(screenContainer);
            if (popupContainer != null)
                RegisterFromTransform(popupContainer);
        }

        private void RegisterFromTransform(Transform root)
        {
            foreach (var screen in root.GetComponentsInChildren<UIScreenBase>(true))
                _screens[screen.ScreenId] = screen;
        }

        public void ShowScreen(UIScreenId id, bool addToHistory = true)
        {
            if (!_screens.TryGetValue(id, out var target)) return;

            if (_current != UIScreenId.None && _screens.TryGetValue(_current, out var current))
            {
                current.gameObject.SetActive(false);
                current.OnScreenHidden();
                if (addToHistory && _current != id)
                    _history.Push(_current);
            }

            _current = id;
            target.gameObject.SetActive(true);
            target.OnScreenShown();

            UpdateChrome(target);
            if (bottomNav != null)
                bottomNav.SetActiveTab(id);
        }

        public void ShowPopup(UIScreenId id)
        {
            if (!_screens.TryGetValue(id, out var popup)) return;

            if (_activePopup != null)
            {
                _activePopup.OnScreenHidden();
                _activePopup.gameObject.SetActive(false);
            }

            _activePopup = popup;
            popup.gameObject.SetActive(true);
            popup.OnScreenShown();
        }

        public void ClosePopup(UIScreenId id)
        {
            if (_activePopup == null || _activePopup.ScreenId != id) return;
            _activePopup.OnScreenHidden();
            _activePopup = null;
        }

        public void GoBack()
        {
            if (_history.Count == 0)
            {
                ShowScreen(UIScreenId.Dashboard, addToHistory: false);
                return;
            }
            var previous = _history.Pop();
            ShowScreen(previous, addToHistory: false);
        }

        public void RefreshTopBar() => topBar?.Refresh();

        private void UpdateChrome(UIScreenBase screen)
        {
            if (topBar != null)
            {
                topBar.gameObject.SetActive(!screen.HideTopBar);
                if (!screen.HideTopBar) topBar.Refresh();
            }

            if (bottomNav != null)
                bottomNav.gameObject.SetActive(!screen.HideBottomNav);
        }

        private void HandleTabSelected(UIScreenId tabId)
        {
            _history.Clear();
            ShowScreen(tabId, addToHistory: false);
        }
    }
}
