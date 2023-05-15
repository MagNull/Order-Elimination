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
    public class CasterCondition : ICommonCondition
    {
        [ShowInInspector, OdinSerialize]
        public List<IEntityCondition> CasterConditions { get; set; } = new();

        public ICommonCondition Clone()
        {
            var clone = new CasterCondition();
            clone.CasterConditions = CasterConditions.Clone();
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor caster)
        {
            return CasterConditions.All(c => c.IsConditionMet(battleContext, caster, caster));
        }
    }
}
