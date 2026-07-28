using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Single row in the leaderboard list.
    /// </summary>
    public class LeaderboardEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Image onlineBadge;

        public void Setup(int rank, string playerName, double score, bool isOnline = false)
        {
            if (rankText != null) rankText.text = $"#{rank}";
            if (nameText != null) nameText.text = playerName;
            if (scoreText != null) scoreText.text = CurrencyFormatter.Format(score);
            if (onlineBadge != null) onlineBadge.enabled = isOnline;
        }
    }
}
