using OrderElimination;

namespace CharacterAbility.BuffEffects
{
    public interface IStatsBuffEffect : ITickEffect
    {
        BattleStats Apply(IBuffTarget target);
        BattleStats Remove(IBuffTarget target);
    }
}