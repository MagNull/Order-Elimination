using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class AbilityRules
    {
        public ICommonCondition[] AvailabilityConditions;
        public ICellCondition[] CellConditions;

        public bool IsAbilityAvailable(IBattleContext battleContext, IAbilitySystemActor caster, Dictionary<ActionPoint, int> abilityCost)
        {
            if (!IsCostAffordableByCaster(abilityCost, caster))
                return false;
            return AvailabilityConditions.All(c => c.IsConditionMet(battleContext, caster));
        }

        public Vector2Int[] GetAvailableCellPositions(IBattleContext battleContext, IAbilitySystemActor caster)
        {
            return battleContext
                .BattleMap
                .Where(cell => CellConditions.All(c => c.IsConditionMet(battleContext, caster, cell)))
                .Select(cell => battleContext.BattleMap.GetCellPosition(cell))
                .ToArray();
        }

        private bool IsCostAffordableByCaster(Dictionary<ActionPoint, int> cost, IAbilitySystemActor caster)
        {
            return cost.Keys.All(actionPoint => cost[actionPoint] <= caster.ActionPoints[actionPoint]);
        }
    }
}
