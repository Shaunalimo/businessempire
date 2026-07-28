using System;
using System.Collections.Generic;
using BusinessEmpire.Core;
using BusinessEmpire.Data;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Random business events that affect income, costs, or trigger narrative popups.
    /// </summary>
    public class EventManager : SingletonMonoBehaviour<EventManager>
    {
        [Serializable]
        public class GameEventDefinition
        {
            public string id;
            public string title;
            [TextArea] public string message;
            public double incomeMultiplier = 1d;
            public double cashBonus;
            public double cashPenalty;
            public int durationDays = 1;
            public float triggerWeight = 1f;
            public BusinessType affectedBusiness = BusinessType.GroceryShop;
            public bool affectsAllBusinesses;
        }

        [SerializeField] private List<GameEventDefinition> eventPool = new List<GameEventDefinition>
        {
            new GameEventDefinition { id = "viral", title = "Viral Moment!", message = "Your business went viral on social media!", incomeMultiplier = 1.5, durationDays = 2, triggerWeight = 1f },
            new GameEventDefinition { id = "inspection", title = "Health Inspection", message = "Pass the inspection to avoid fines.", cashPenalty = 500, durationDays = 1, triggerWeight = 0.8f },
            new GameEventDefinition { id = "supplier", title = "Supplier Deal", message = "A bulk supplier offered a great deal.", cashBonus = 2000, durationDays = 1, triggerWeight = 1.2f },
            new GameEventDefinition { id = "strike", title = "Employee Strike", message = "Employees demand better conditions.", incomeMultiplier = 0.7, durationDays = 3, triggerWeight = 0.5f },
            new GameEventDefinition { id = "boom", title = "Economic Boom", message = "The market is thriving! All businesses benefit.", incomeMultiplier = 1.25, durationDays = 5, affectsAllBusinesses = true, triggerWeight = 0.6f }
        };

        private readonly Dictionary<string, ActiveEvent> _activeEvents = new Dictionary<string, ActiveEvent>();

        private struct ActiveEvent
        {
            public GameEventDefinition definition;
            public int daysRemaining;
        }

        [SerializeField] private float dailyEventChance = 0.35f;

        public void RollDailyEvents()
        {
            if (UnityEngine.Random.value > dailyEventChance) return;

            var evt = PickRandomEvent();
            if (evt == null) return;

            ActivateEvent(evt);
            GameEvents.RaiseBusinessEvent(evt.id, evt.message);
        }

        public void ProcessActiveEvents()
        {
            var keys = new List<string>(_activeEvents.Keys);
            foreach (var key in keys)
            {
                var active = _activeEvents[key];
                active.daysRemaining--;
                if (active.daysRemaining <= 0)
                    _activeEvents.Remove(key);
                else
                    _activeEvents[key] = active;
            }
        }

        public double GetBusinessMultiplier(BusinessType type)
        {
            double multiplier = 1d;
            foreach (var kvp in _activeEvents)
            {
                var evt = kvp.Value.definition;
                if (evt.affectsAllBusinesses || evt.affectedBusiness == type)
                    multiplier *= evt.incomeMultiplier;
            }
            return multiplier;
        }

        private GameEventDefinition PickRandomEvent()
        {
            if (eventPool.Count == 0) return null;

            float totalWeight = 0f;
            foreach (var e in eventPool) totalWeight += e.triggerWeight;

            float roll = UnityEngine.Random.Range(0f, totalWeight);
            float cumulative = 0f;
            foreach (var e in eventPool)
            {
                cumulative += e.triggerWeight;
                if (roll <= cumulative) return e;
            }

            return eventPool[eventPool.Count - 1];
        }

        private void ActivateEvent(GameEventDefinition evt)
        {
            _activeEvents[evt.id] = new ActiveEvent { definition = evt, daysRemaining = evt.durationDays };

            if (evt.cashBonus > 0) EconomyManager.Instance.AddCash(evt.cashBonus);
            if (evt.cashPenalty > 0) EconomyManager.Instance.TrySpendCash(evt.cashPenalty);
        }
    }
}
