﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class AbilityRules
    {
        public readonly List<ICommonCondition> AvailabilityConditions = new();
        public readonly List<ICellCondition> CellConditions = new();//TODO Critical move to targeting system
        public readonly Dictionary<ActionPoint, int> UsageCost = new();

        public AbilityRules(
            IEnumerable<ICommonCondition> availabilityConditions, 
            IEnumerable<ICellCondition> cellConditions, 
            IReadOnlyDictionary<ActionPoint, int> usageCost)
        {
            AvailabilityConditions = availabilityConditions.ToList();
            if (cellConditions != null)
                CellConditions = cellConditions.ToList();
            UsageCost = usageCost.ToDictionary(e => e.Key, e => e.Value);
        }

        public bool IsAbilityAvailable(IBattleContext battleContext, AbilitySystemActor caster)
        {
            if (!IsCostAffordableByCaster(caster))
                return false;
            return AvailabilityConditions.All(c => c.IsConditionMet(battleContext, caster));
        }

        public Vector2Int[] GetAvailableCellPositions(IBattleContext battleContext, AbilitySystemActor caster)
        {
            return battleContext
                .BattleMap
                .CellRangeBorders
                .EnumerateCellPositions()
                .Where(pos => CellConditions.All(c => c.IsConditionMet(battleContext, caster, pos)))
                .ToArray();
        }

        private bool IsCostAffordableByCaster(AbilitySystemActor caster)
        {
            return UsageCost.Keys.All(actionPoint => UsageCost[actionPoint] <= caster.ActionPoints[actionPoint]);
        }
    }
}