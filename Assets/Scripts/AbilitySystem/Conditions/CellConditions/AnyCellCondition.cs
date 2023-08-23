using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class AnyCellCondition : ICellCondition
    {
        [ShowInInspector, OdinSerialize]
        public ICellCondition[] CellConditions { get; private set; }

        public ICellCondition Clone()
        {
            var clone = new AnyCellCondition();
            clone.CellConditions = CloneableCollectionsExtensions.DeepClone(CellConditions);
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, Vector2Int positionToCheck)
        {
            return CellConditions.Any(c => c.IsConditionMet(battleContext, askingEntity, positionToCheck));
        }
    }
}
