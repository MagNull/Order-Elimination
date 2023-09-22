using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class AbilityRules
    {
        private readonly Dictionary<EnergyPoint, int> _usageCost = new();
        private readonly List<ICommonCondition> _availabilityConditions = new();

        public IReadOnlyList<ICommonCondition> AvailabilityConditions => _availabilityConditions;
        public IReadOnlyDictionary<EnergyPoint, int> UsageCost => _usageCost;

        public AbilityRules(
            IEnumerable<ICommonCondition> availabilityConditions, 
            IReadOnlyDictionary<EnergyPoint, int> usageCost)
        {
            _availabilityConditions = availabilityConditions.ToList();
            _usageCost = usageCost.ToDictionary(e => e.Key, e => e.Value);
        }

        public bool IsAbilityAvailable(IBattleContext battleContext, AbilitySystemActor caster)
        {
            if (!IsCostAffordableByCaster(caster))
                return false;
            return AvailabilityConditions.AllMet(battleContext, caster);
        }

        private bool IsCostAffordableByCaster(AbilitySystemActor caster)
        {
            return UsageCost.Keys.All(actionPoint => UsageCost[actionPoint] <= caster.EnergyPoints[actionPoint]);
        }
    }
}
