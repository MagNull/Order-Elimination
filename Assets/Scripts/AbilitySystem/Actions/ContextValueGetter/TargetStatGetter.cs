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

        public bool CanBePrecalculatedWith(ValueCalculationContext context)
        {
            return context.BattleTarget != null;
        }

        public IContextValueGetter Clone()
        {
            var clone = new TargetStatGetter();
            clone.TargetStat = TargetStat;
            clone.UseUnmodifiedValue = UseUnmodifiedValue;
            return clone;
        }

        public float GetValue(ValueCalculationContext context)
        {
            if (!CanBePrecalculatedWith(context))
                throw new NotEnoughDataArgumentException();
            if (!UseUnmodifiedValue)
                return context.BattleTarget.BattleStats[TargetStat].ModifiedValue;
            else
                return context.BattleTarget.BattleStats[TargetStat].UnmodifiedValue;
        }
    }
}
