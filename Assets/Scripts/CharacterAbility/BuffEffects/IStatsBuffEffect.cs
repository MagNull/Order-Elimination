using OrderElimination;

namespace CharacterAbility.BuffEffects
{
    public interface IStatsBuffEffect : ITickEffect
    {
        BattleStats Apply();
        BattleStats Remove();
    }
}