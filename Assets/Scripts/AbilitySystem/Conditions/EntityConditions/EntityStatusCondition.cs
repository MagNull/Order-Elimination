using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class EntityStatusCondition : IEntityCondition
    {
        [ShowInInspector, OdinSerialize]
        public BattleStatus BattleStatus { get; set; }

        [ShowInInspector, OdinSerialize]
        public bool HasStatus { get; set; }

        public IEntityCondition Clone()
        {
            var clone = new EntityStatusCondition();
            clone.BattleStatus = BattleStatus;
            clone.HasStatus = HasStatus;
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entityToCheck)
        {
            return entityToCheck.StatusHolder.HasStatus(BattleStatus) == HasStatus;
        }
    }
}
