using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class EntityFilterCondition : ITargetCondition
    {
        [ShowInInspector, OdinSerialize]
        public EntityFilter EntityFilter { get; private set; } = new EntityFilter();

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor caster, AbilitySystemActor target)
        {
            return EntityFilter.IsAllowed(battleContext, caster, target);
        }
    }
}
