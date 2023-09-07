using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
    [Serializable]
    public struct TargetStatGetter : IContextValueGetter
    {
        [OdinSerialize]
        public BattleStat TargetStat { get; private set; }

        [OdinSerialize]
        public bool UseUnmodifiedValue { get; private set; }

        public string DisplayedFormula => $"Target.{TargetStat}({(UseUnmodifiedValue ? "orig" : "mod")})";

        public IContextValueGetter Clone()
        {
            var clone = new TargetStatGetter();
            clone.TargetStat = TargetStat;
            clone.UseUnmodifiedValue = UseUnmodifiedValue;
            return clone;
        }

        public float GetValue(ValueCalculationContext context)
        {
            if (context.Target == null
                || !context.Target.BattleStats.HasParameter(TargetStat))
                return 0;
            if (!UseUnmodifiedValue)
                return context.Target.BattleStats[TargetStat].ModifiedValue;
            else
                return context.Target.BattleStats[TargetStat].UnmodifiedValue;
        }
    }
}
