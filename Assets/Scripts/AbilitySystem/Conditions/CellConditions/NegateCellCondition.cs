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
        {
            return !CellCondition.IsConditionMet(battleContext, askingEntity, positionToCheck);
        }
    }
}
