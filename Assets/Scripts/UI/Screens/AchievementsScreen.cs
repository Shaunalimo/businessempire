using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI.Screens
{
    /// <summary>
    /// Screen 15: Achievements — progression milestones.
    /// </summary>
    public class AchievementsScreen : UIScreenBase
    {
        [SerializeField] private Transform listContainer;
        [SerializeField] private UIListRowView rowPrefab;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Button backButton;

        private void Awake()
        {
            screenId = UIScreenId.Achievements;
            backButton?.onClick.AddListener(GoBack);
        }

        public override void Refresh()
        {
            int unlocked = 0;
            ClearList();
            foreach (var a in UIDummyDataProvider.GetAchievements())
            {
                if (a.unlocked) unlocked++;
                var row = Instantiate(rowPrefab, listContainer);
                row.Bind(a.title, a.description, a.unlocked ? "Unlocked" : $"+{a.gemReward} gems",
                    null, a.unlocked ? UITheme.AccentSuccess : UITheme.TextMuted);
            }
            if (progressText != null) progressText.text = $"{unlocked} / {UIDummyDataProvider.GetAchievements().Count} unlocked";
        }

        private void ClearList()
        {
            if (listContainer == null) return;
            foreach (Transform c in listContainer) Destroy(c.gameObject);
        }
    }
}
