using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class AnyCellCondition : ICellCondition
    {
        [ShowInInspector, OdinSerialize]
        public ICellCondition[] CellConditions { get; private set; } = new ICellCondition[0];

        public ICellCondition Clone()
        {
            var clone = new AnyCellCondition();
            clone.CellConditions = CellConditions.DeepClone();
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, Vector2Int positionToCheck)
        {
            return CellConditions.Any(c => c.IsConditionMet(battleContext, askingEntity, positionToCheck));
        }
    }
}
