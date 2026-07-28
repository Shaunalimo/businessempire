using BusinessEmpire.Core;
using TMPro;
using UnityEngine;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Toast notification for random business events and achievements.
    /// </summary>
    public class EventNotificationUI : MonoBehaviour
    {
        [SerializeField] private UIAnimator animator;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private float displayDuration = 3f;

        private float _timer;

        private void OnEnable()
        {
            GameEvents.OnBusinessEventTriggered += ShowEvent;
            GameEvents.OnAchievementUnlocked += ShowAchievement;
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            GameEvents.OnBusinessEventTriggered -= ShowEvent;
            GameEvents.OnAchievementUnlocked -= ShowAchievement;
        }

        private void Update()
        {
            if (!gameObject.activeSelf) return;
            _timer -= Time.deltaTime;
            if (_timer <= 0f) animator?.Hide();
        }

        private void ShowEvent(string id, string message)
        {
            titleText.text = "Business Event";
            messageText.text = message;
            Present();
        }

        private void ShowAchievement(string id)
        {
            titleText.text = "Achievement Unlocked!";
            messageText.text = id;
            Present();
        }

        private void Present()
        {
            _timer = displayDuration;
            animator?.Show();
        }
    }
}
