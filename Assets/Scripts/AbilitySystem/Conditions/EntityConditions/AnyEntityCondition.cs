using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class AnyEntityCondition : IEntityCondition
    {
        [ShowInInspector, OdinSerialize]
        public IEntityCondition[] EntityConditions { get; private set; } = new IEntityCondition[0];

        public IEntityCondition Clone()
        {
            var clone = new AnyEntityCondition();
            clone.EntityConditions = EntityConditions.DeepClone();
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entityToCheck)
        {
            return EntityConditions.Any(c => c.IsConditionMet(battleContext, askingEntity, entityToCheck));
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entityToCheck, CellGroupsContainer cellGroups)
        {
            return EntityConditions.Any(c => c.IsConditionMet(battleContext, askingEntity, entityToCheck, cellGroups));
        }
    }
}
