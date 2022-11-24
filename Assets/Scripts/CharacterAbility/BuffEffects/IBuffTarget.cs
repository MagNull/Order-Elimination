using OrderElimination;

namespace CharacterAbility.BuffEffects
{
    public interface IBuffTarget
    {
        public IReadOnlyBattleStats Stats { get; }
    }
}