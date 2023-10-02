using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    [GUIColor(1, 1, 0)]
    public interface ICellCondition : ICloneable<ICellCondition>
    {
        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, Vector2Int positionToCheck);
        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, Vector2Int positionToCheck, CellGroupsContainer cellGroups);

        //For cluster-check optimizations
        public Vector2Int[] FilterMany(IBattleContext battleContext, AbilitySystemActor askingEntity, IEnumerable<Vector2Int> positions)
            => FilterMany(battleContext, askingEntity, positions, null);

        public virtual Vector2Int[] FilterMany(IBattleContext battleContext, AbilitySystemActor askingEntity, IEnumerable<Vector2Int> positions, CellGroupsContainer cellGroups)
        {
            return positions.Where(p => IsConditionMet(battleContext, askingEntity, p, cellGroups)).ToArray();
        }
    }
}
