using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Scene anchor for save/load — ensures SaveManager exists when Gameplay loads directly.
    /// </summary>
    public class SaveLoadManager : MonoBehaviour
    {
        private void Awake()
        {
            _ = SaveManager.Instance;
        }
    }
}
