using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class EntityPositionCondition : IEntityCondition
    {
        [ShowInInspector, OdinSerialize]
        public ICellCondition[] PositionConditions { get; private set; } = new ICellCondition[0];

        public IEntityCondition Clone()
        {
            var clone = new EntityPositionCondition();
            clone.PositionConditions = PositionConditions.DeepClone();
            return clone;
        }

        public bool IsConditionMet(
            IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entityToCheck)
        {
            return PositionConditions.AllMet(battleContext, askingEntity, entityToCheck.Position);
        }
    }
}
