using OrderElimination;

namespace CharacterAbility.BuffEffects
{
    public interface IStatsBuffEffect : ITickEffect
    {
        BattleStats Apply(IBattleObject target);
        BattleStats Remove(IBattleObject target);
    }
}