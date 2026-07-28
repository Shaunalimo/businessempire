using UnityEngine;

namespace BusinessEmpire.Core
{
    /// <summary>
    /// Bootstrap scene entry — initializes persistent managers on awake.
    /// </summary>
    public class BootstrapManager : MonoBehaviour
    {
        [SerializeField] private bool initializeOnAwake = true;

        private void Awake()
        {
            if (initializeOnAwake)
                GameBootstrap.EnsureManagersExist();
        }
    }
}
