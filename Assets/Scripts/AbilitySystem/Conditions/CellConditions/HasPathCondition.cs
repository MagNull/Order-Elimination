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

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, Vector2Int positionToCheck)
        {
            var borders = battleContext.BattleMap.CellRangeBorders;
            var hasPath = Pathfinding.PathExists(askingEntity.Position, positionToCheck, borders, CellPredicate, out var path);
            if (!hasPath) return false;
            if (LimitByCasterMovement)
            {
                if (!askingEntity.BattleStats.HasParameter(BattleStat.MaxMovementDistance))
                    Logging.LogException( new ArgumentException("Caster doesn't have movement parameter."));
                if (path.Length > askingEntity.BattleStats[BattleStat.MaxMovementDistance].ModifiedValue)
                    return false;
            }
            return true;

            bool CellPredicate(Vector2Int p) => PathConditions.All(c => c.IsConditionMet(battleContext, askingEntity, p));
        }
    }
}
