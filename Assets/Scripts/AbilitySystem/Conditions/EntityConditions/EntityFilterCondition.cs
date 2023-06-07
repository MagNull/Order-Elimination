using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination.AbilitySystem
{
    public class EntityFilterCondition : IEntityCondition
    {
        [ShowInInspector, OdinSerialize]
        public EntityFilter EntityFilter { get; private set; } = new EntityFilter();

        public IEntityCondition Clone()
        {
            var clone = new EntityFilterCondition();
            clone.EntityFilter = EntityFilter.Clone();
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entityToCheck)
        {
            return EntityFilter.IsAllowed(battleContext, askingEntity, entityToCheck);
        }
    }
}
