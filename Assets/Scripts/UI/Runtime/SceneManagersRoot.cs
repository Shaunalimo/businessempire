using BusinessEmpire.Core;
using BusinessEmpire.Managers;
using BusinessEmpire.Monetization;
using UnityEngine;

namespace BusinessEmpire.UI.Runtime
{
    /// <summary>
    /// Visible scene anchor that initializes all manager singletons on load.
    /// </summary>
    public class SceneManagersRoot : MonoBehaviour
    {
        [SerializeField] private bool initializeOnAwake = true;

        private void Awake()
        {
            if (!initializeOnAwake) return;
            GameBootstrap.EnsureManagersExist();
            EnsureNamedChild("GameManager", GameManager.Instance);
            EnsureNamedChild("EconomyManager", EconomyManager.Instance);
            EnsureNamedChild("UIManager", UIManager.Instance);
            EnsureNamedChild("MonetizationManager", MonetizationManager.Instance);
        }

        private static void EnsureNamedChild(string label, Object manager)
        {
            if (manager == null) return;
            var mb = manager as MonoBehaviour;
            if (mb != null) mb.gameObject.name = label;
        }
    }
}
