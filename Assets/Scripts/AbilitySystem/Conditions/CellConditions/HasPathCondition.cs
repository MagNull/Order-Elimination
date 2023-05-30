using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class HasPathCondition : ICellCondition
    {
        [ShowInInspector, OdinSerialize]
        public bool LimitByCasterMovement { get; private set; }

        [ShowInInspector, OdinSerialize]
        public ICellCondition[] PathConditions { get; private set; }

        public ICellCondition Clone()
        {
            var clone = new HasPathCondition();
            clone.LimitByCasterMovement = LimitByCasterMovement;
            clone.PathConditions = PathConditions;
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor caster, Vector2Int cellPosition)
        {
            var borders = battleContext.BattleMap.CellRangeBorders;
            var hasPath = Pathfinding.PathExists(caster.Position, cellPosition, borders, CellPredicate, out var path);
            if (!hasPath) return false;
            if (LimitByCasterMovement)
            {
                if (!caster.BattleStats.HasParameter(BattleStat.MaxMovementDistance))
                    throw new ArgumentException("Caster doesn't have movement parameter.");
                if (path.Length > caster.BattleStats[BattleStat.MaxMovementDistance].ModifiedValue)
                    return false;
            }
            return true;

            bool CellPredicate(Vector2Int p) => PathConditions.All(c => c.IsConditionMet(battleContext, caster, p));
        }
    }
}
