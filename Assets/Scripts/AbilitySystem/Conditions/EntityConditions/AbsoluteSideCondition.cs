using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination.AbilitySystem.Conditions
{
    public class AbsoluteSideCondition : IEntityCondition
    {
        [ShowInInspector, OdinSerialize]
        public EnumMask<BattleSide> AllowedSides { get; private set; } = EnumMask<BattleSide>.Full;

        public IEntityCondition Clone()
        {
            var clone = new AbsoluteSideCondition();
            clone.AllowedSides = AllowedSides.Clone();
            return clone;
        }

        public bool IsConditionMet(
            IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entityToCheck)
        {
            return AllowedSides[entityToCheck.BattleSide];
        }
    }
}
