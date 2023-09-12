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

        //For cluster-check optimizations
        public virtual Vector2Int[] FilterMany(IBattleContext battleContext, AbilitySystemActor askingEntity, IEnumerable<Vector2Int> positions)
        {
            return positions.Where(p => IsConditionMet(battleContext, askingEntity, p)).ToArray();
        }
    }
}
