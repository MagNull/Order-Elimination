using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class AnyCommonCondition : ICommonCondition
    {
        [ShowInInspector, OdinSerialize]
        public ICommonCondition[] CommonConditions { get; private set; } = new ICommonCondition[0];

        public ICommonCondition Clone()
        {
            var clone = new AnyCommonCondition();
            clone.CommonConditions = CommonConditions.DeepClone();
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity)
        {
            return CommonConditions.Any(c => c.IsConditionMet(battleContext, askingEntity));
        }
        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, CellGroupsContainer cellGroups)
        {
            return CommonConditions.Any(c => c.IsConditionMet(battleContext, askingEntity, cellGroups));
        }
    }
}
