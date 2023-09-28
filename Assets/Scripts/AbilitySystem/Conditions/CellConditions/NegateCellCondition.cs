using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class NegateCellCondition : ICellCondition
    {
        [ShowInInspector, OdinSerialize]
        public ICellCondition CellCondition { get; set; }

        public ICellCondition Clone()
        {
            var clone = new NegateCellCondition();
            clone.CellCondition = CellCondition.Clone();
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, Vector2Int positionToCheck)
            => IsConditionMet(battleContext, askingEntity, positionToCheck, null);

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, Vector2Int positionToCheck, CellGroupsContainer cellGroups)
        {
            return !CellCondition.IsConditionMet(battleContext, askingEntity, positionToCheck, cellGroups);
        }
    }
}
