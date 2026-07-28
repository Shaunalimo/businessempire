using System;
using System.Collections.Generic;
using BusinessEmpire.Core;
using BusinessEmpire.Data;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Marketing campaigns that boost business income for a limited time.
    /// </summary>
    public class MarketingManager : SingletonMonoBehaviour<MarketingManager>
    {
        [Serializable]
        public struct CampaignTemplate
        {
            public string id;
            public string name;
            public double cost;
            public double incomeBonus;
            public int durationDays;
        }

        [SerializeField] private List<CampaignTemplate> campaigns = new List<CampaignTemplate>
        {
            new CampaignTemplate { id = "social", name = "Social Media Blitz", cost = 2500, incomeBonus = 1.15, durationDays = 3 },
            new CampaignTemplate { id = "tv", name = "TV Commercial", cost = 15000, incomeBonus = 1.3, durationDays = 7 },
            new CampaignTemplate { id = "influencer", name = "Influencer Partnership", cost = 50000, incomeBonus = 1.5, durationDays = 14 }
        };

        public IReadOnlyList<CampaignTemplate> Campaigns => campaigns;

        public bool TryLaunchCampaign(string campaignId, BusinessType target)
        {
            var template = campaigns.Find(c => c.id == campaignId);
            if (string.IsNullOrEmpty(template.id)) return false;

            var instance = BusinessManager.Instance.GetInstance(target);
            if (instance == null || !instance.isOwned) return false;
            if (!EconomyManager.Instance.TrySpendCash(template.cost)) return false;

            var campaign = new MarketingCampaignData
            {
                campaignId = Guid.NewGuid().ToString("N"),
                targetBusiness = target,
                incomeBonusMultiplier = template.incomeBonus,
                daysRemaining = template.durationDays
            };

            SaveManager.Instance.CurrentSave.activeCampaigns.Add(campaign);
            return true;
        }

        public double GetBusinessBonus(BusinessType type)
        {
            double bonus = 1d;
            foreach (var campaign in SaveManager.Instance.CurrentSave.activeCampaigns)
            {
                if (campaign.targetBusiness == type)
                    bonus *= campaign.incomeBonusMultiplier;
            }
            return bonus;
        }

        public void ProcessDailyCampaigns()
        {
            var save = SaveManager.Instance.CurrentSave;
            for (int i = save.activeCampaigns.Count - 1; i >= 0; i--)
            {
                save.activeCampaigns[i].daysRemaining--;
                if (save.activeCampaigns[i].daysRemaining <= 0)
                    save.activeCampaigns.RemoveAt(i);
            }
        }
    }
}
