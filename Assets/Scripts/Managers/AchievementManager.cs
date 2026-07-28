using System;
using System.Collections.Generic;
using BusinessEmpire.Core;
using BusinessEmpire.Data;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Achievement tracking and unlock rewards.
    /// </summary>
    public class AchievementManager : SingletonMonoBehaviour<AchievementManager>
    {
        [Serializable]
        public struct AchievementDefinition
        {
            public string id;
            public string title;
            public string description;
            public int gemReward;
            public AchievementType type;
            public double targetValue;
        }

        public enum AchievementType
        {
            TotalCash,
            BusinessesOwned,
            TotalProfit,
            EmployeesHired,
            DaysPlayed,
            ResearchCompleted
        }

        [SerializeField] private List<AchievementDefinition> achievements = new List<AchievementDefinition>
        {
            new AchievementDefinition { id = "first_business", title = "Entrepreneur", description = "Own your first business", type = AchievementType.BusinessesOwned, targetValue = 1, gemReward = 5 },
            new AchievementDefinition { id = "millionaire", title = "Millionaire", description = "Accumulate $1,000,000", type = AchievementType.TotalCash, targetValue = 1000000, gemReward = 25 },
            new AchievementDefinition { id = "empire", title = "Business Empire", description = "Own all 8 businesses", type = AchievementType.BusinessesOwned, targetValue = 8, gemReward = 100 },
            new AchievementDefinition { id = "workforce", title = "Workforce Leader", description = "Hire 50 employees total", type = AchievementType.EmployeesHired, targetValue = 50, gemReward = 20 },
            new AchievementDefinition { id = "veteran", title = "Veteran CEO", description = "Play for 30 days", type = AchievementType.DaysPlayed, targetValue = 30, gemReward = 15 }
        };

        public IReadOnlyList<AchievementDefinition> AllAchievements => achievements;

        public void CheckAchievements()
        {
            var save = SaveManager.Instance.CurrentSave;

            foreach (var achievement in achievements)
            {
                if (save.unlockedAchievements.Contains(achievement.id)) continue;
                if (!IsAchievementMet(achievement, save)) continue;

                save.unlockedAchievements.Add(achievement.id);
                EconomyManager.Instance.AddPremium(achievement.gemReward);
                GameEvents.RaiseAchievement(achievement.id);
            }
        }

        private bool IsAchievementMet(AchievementDefinition achievement, GameSaveData save)
        {
            double current = achievement.type switch
            {
                AchievementType.TotalCash => save.cash,
                AchievementType.BusinessesOwned => save.businessesOwned,
                AchievementType.TotalProfit => save.totalLifetimeProfit,
                AchievementType.EmployeesHired => save.totalEmployeesHired,
                AchievementType.DaysPlayed => save.totalDaysPlayed,
                AchievementType.ResearchCompleted => save.completedResearch.Count,
                _ => 0d
            };

            return current >= achievement.targetValue;
        }

        public bool IsUnlocked(string achievementId) =>
            SaveManager.Instance.CurrentSave.unlockedAchievements.Contains(achievementId);
    }
}
