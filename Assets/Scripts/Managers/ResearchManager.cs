using System;
using System.Collections.Generic;
using BusinessEmpire.Core;
using BusinessEmpire.Data;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Research and Development tree unlocking global income bonuses.
    /// </summary>
    public class ResearchManager : SingletonMonoBehaviour<ResearchManager>
    {
        [Serializable]
        public struct ResearchNode
        {
            public string id;
            public string name;
            public string description;
            public double cost;
            public double globalIncomeBonus;
            public int requiredPlayerLevel;
        }

        [SerializeField] private List<ResearchNode> researchTree = new List<ResearchNode>
        {
            new ResearchNode { id = "automation", name = "Process Automation", description = "+5% global income", cost = 10000, globalIncomeBonus = 0.05, requiredPlayerLevel = 1 },
            new ResearchNode { id = "analytics", name = "Business Analytics", description = "+10% global income", cost = 50000, globalIncomeBonus = 0.10, requiredPlayerLevel = 3 },
            new ResearchNode { id = "ai", name = "AI Optimization", description = "+20% global income", cost = 200000, globalIncomeBonus = 0.20, requiredPlayerLevel = 5 }
        };

        public IReadOnlyList<ResearchNode> ResearchTree => researchTree;

        public double GlobalIncomeMultiplier
        {
            get
            {
                double bonus = 1d;
                foreach (var completed in SaveManager.Instance.CurrentSave.completedResearch)
                {
                    if (completed.isCompleted)
                        bonus += completed.globalIncomeBonus;
                }
                return bonus;
            }
        }

        public bool IsResearchCompleted(string researchId)
        {
            return SaveManager.Instance.CurrentSave.completedResearch
                .Exists(r => r.researchId == researchId && r.isCompleted);
        }

        public bool TryPurchaseResearch(string researchId)
        {
            var node = researchTree.Find(r => r.id == researchId);
            if (string.IsNullOrEmpty(node.id) || IsResearchCompleted(researchId)) return false;

            var save = SaveManager.Instance.CurrentSave;
            if (save.playerLevel < node.requiredPlayerLevel) return false;
            if (!EconomyManager.Instance.TrySpendCash(node.cost)) return false;

            save.completedResearch.Add(new ResearchData
            {
                researchId = node.id,
                name = node.name,
                isCompleted = true,
                globalIncomeBonus = node.globalIncomeBonus
            });

            return true;
        }
    }
}
