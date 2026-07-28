using BusinessEmpire.Core;
using BusinessEmpire.UI;
using UnityEngine;

namespace BusinessEmpire.UI.Runtime
{
    /// <summary>
    /// Builds the complete playable mobile UI at runtime when Gameplay.unity loads.
    /// Skipped when the scene already contains editor-built UI.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class PlayablePrototypeBuilder : MonoBehaviour
    {
        private void Awake()
        {
            if (Object.FindObjectOfType<Canvas>() != null)
            {
                Destroy(this);
                return;
            }

            RuntimeUIFactory.EnsureEventSystem();
            GameBootstrap.EnsureManagersExist();
            UIGameDataBridge.UseDummyData = false;

            _ = UIManager.Instance;
            var canvas = RuntimeUIFactory.CreateCanvas("MobileUI");
            PlayableUILayoutBuilder.BuildGameplayInterface(canvas.transform);
            new GameObject("UIManager").AddComponent<UIManager>();
            new GameObject("SceneManagersRoot").AddComponent<SceneManagersRoot>();
        }
    }
}
