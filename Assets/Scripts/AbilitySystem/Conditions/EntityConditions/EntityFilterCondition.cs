using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor caster, AbilitySystemActor entity)
        {
            return EntityFilter.IsAllowed(battleContext, caster, entity);
        }
    }
}
