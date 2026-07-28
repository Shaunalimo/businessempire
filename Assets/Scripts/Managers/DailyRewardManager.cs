using System;
using System.Collections.Generic;
using BusinessEmpire.Core;
using BusinessEmpire.Data;
using BusinessEmpire.Utils;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Daily login rewards with 7-day cycle and streak tracking.
    /// </summary>
    public class DailyRewardManager : SingletonMonoBehaviour<DailyRewardManager>
    {
        [Serializable]
        public struct DailyReward
        {
            public int day;
            public double cashReward;
            public int gemReward;
        }

        [SerializeField] private List<DailyReward> rewardCycle = new List<DailyReward>
        {
            new DailyReward { day = 1, cashReward = 1000, gemReward = 1 },
            new DailyReward { day = 2, cashReward = 2000, gemReward = 2 },
            new DailyReward { day = 3, cashReward = 3500, gemReward = 3 },
            new DailyReward { day = 4, cashReward = 5000, gemReward = 5 },
            new DailyReward { day = 5, cashReward = 7500, gemReward = 7 },
            new DailyReward { day = 6, cashReward = 10000, gemReward = 10 },
            new DailyReward { day = 7, cashReward = 25000, gemReward = 25 }
        };

        public bool CanClaimToday
        {
            get
            {
                var save = SaveManager.Instance.CurrentSave;
                return TimeHelper.IsNewUtcDaySince(save.lastDailyRewardClaimTimestamp);
            }
        }

        public int CurrentRewardDay => SaveManager.Instance.CurrentSave.dailyRewardIndex + 1;

        public void CheckLoginStreak()
        {
            var save = SaveManager.Instance.CurrentSave;
            if (save.lastDailyRewardClaimTimestamp <= 0) return;

            int daysSinceClaim = TimeHelper.DaysSince(save.lastDailyRewardClaimTimestamp);
            if (daysSinceClaim > 1)
            {
                save.consecutiveLoginDays = 0;
                save.dailyRewardIndex = 0;
            }
        }

        public bool TryClaimDailyReward()
        {
            if (!CanClaimToday) return false;

            var save = SaveManager.Instance.CurrentSave;
            int index = save.dailyRewardIndex % GameConstants.DailyRewardCycleDays;
            var reward = rewardCycle[index];

            EconomyManager.Instance.AddCash(reward.cashReward);
            EconomyManager.Instance.AddPremium(reward.gemReward);

            save.lastDailyRewardClaimTimestamp = TimeHelper.UnixNow;
            save.consecutiveLoginDays++;
            save.dailyRewardIndex = (save.dailyRewardIndex + 1) % GameConstants.DailyRewardCycleDays;

            GameEvents.RaiseDailyRewardClaimed();
            return true;
        }

        public DailyReward GetCurrentRewardPreview()
        {
            int index = SaveManager.Instance.CurrentSave.dailyRewardIndex % rewardCycle.Count;
            return rewardCycle[index];
        }
    }
}
