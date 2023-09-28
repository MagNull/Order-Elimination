using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination.AbilitySystem
{
    public class EntityStatusCondition : IEntityCondition
    {
        public enum StatusRequireType
        {
            HasStatus,
            NoStatus
        }

        [ShowInInspector, OdinSerialize]
        public BattleStatus BattleStatus { get; set; }

        [ShowInInspector, OdinSerialize]
        public StatusRequireType RequireType { get; set; }

        public IEntityCondition Clone()
        {
            var clone = new EntityStatusCondition();
            clone.BattleStatus = BattleStatus;
            clone.RequireType = RequireType;
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entityToCheck)
        {
            var hasStatus = entityToCheck.StatusHolder.HasStatus(BattleStatus);
            return RequireType switch
            {
                StatusRequireType.HasStatus => hasStatus,
                StatusRequireType.NoStatus => !hasStatus,
                _ => throw new System.NotImplementedException(),
            };
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entityToCheck, CellGroupsContainer cellGroups)
            => IsConditionMet(battleContext, askingEntity, entityToCheck);
    }
}
