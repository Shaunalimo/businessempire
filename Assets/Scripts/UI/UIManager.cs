using BusinessEmpire.Core;
using UnityEngine;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Central UI coordinator — refreshes HUD on economy events.
    /// Attach to scene as "UIManager".
    /// </summary>
    public class UIManager : SingletonMonoBehaviour<UIManager>
    {
        protected override void OnSingletonAwake()
        {
            GameEvents.OnCashChanged += OnEconomyChanged;
            GameEvents.OnPremiumCurrencyChanged += OnPremiumChanged;
            GameEvents.OnDayAdvanced += OnDayAdvanced;
            GameEvents.OnGameLoaded += OnGameLoaded;
        }

        protected override void OnDestroy()
        {
            GameEvents.OnCashChanged -= OnEconomyChanged;
            GameEvents.OnPremiumCurrencyChanged -= OnPremiumChanged;
            GameEvents.OnDayAdvanced -= OnDayAdvanced;
            GameEvents.OnGameLoaded -= OnGameLoaded;
            base.OnDestroy();
        }

        private void OnEconomyChanged(double _) => Refresh();
        private void OnPremiumChanged(int _) => Refresh();
        private void OnDayAdvanced() => Refresh();
        private void OnGameLoaded() => Refresh();

        public void Refresh() => UINavigationManager.Instance?.RefreshTopBar();
    }
}
