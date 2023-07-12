using OrderElimination.Infrastructure;
using System;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleStats
    {
        public event Action<BattleStat> StatsChanged;
        public bool HasParameter(BattleStat battleStat);
        public ProcessingParameter<float> this[BattleStat battleStat] { get; }
    }
}
