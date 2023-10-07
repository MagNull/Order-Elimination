using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
    public class NegateEntityCondition : IEntityCondition
    {
        [ShowInInspector, OdinSerialize]
        public IEntityCondition EntityCondition { get; set; }

        public IEntityCondition Clone()
        {
            var clone = new NegateEntityCondition();
            clone.EntityCondition = EntityCondition.Clone();
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entityToCheck)
        {
            return !EntityCondition.IsConditionMet(battleContext, askingEntity, entityToCheck);
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entityToCheck, CellGroupsContainer cellGroups)
        {
            return !EntityCondition.IsConditionMet(battleContext, askingEntity, entityToCheck, cellGroups);
        }
    }
}
