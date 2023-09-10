using OrderElimination.Infrastructure;
using System;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleStats //Make ICloneable?
    {
        public event Action<BattleStat> StatsChanged;
        public bool HasParameter(BattleStat battleStat);//Unnecessary: entities always have all stats
        public ProcessingParameter<float> this[BattleStat battleStat] { get; }
    }
}
