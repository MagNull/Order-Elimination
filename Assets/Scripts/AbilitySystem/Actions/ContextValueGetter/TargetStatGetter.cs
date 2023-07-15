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

        public string DisplayedFormula => $"Caster.{TargetStat}({(UseUnmodifiedValue ? "orig" : "mod")})";

        public IContextValueGetter Clone()
        {
            var clone = new TargetStatGetter();
            clone.TargetStat = TargetStat;
            clone.UseUnmodifiedValue = UseUnmodifiedValue;
            return clone;
        }

        public float GetValue(ActionContext useContext)
        {
            if (useContext.ActionTarget == null
                || !useContext.ActionTarget.BattleStats.HasParameter(TargetStat))
                return 0;
            if (!UseUnmodifiedValue)
                return useContext.ActionTarget.BattleStats[TargetStat].ModifiedValue;
            else
                return useContext.ActionTarget.BattleStats[TargetStat].UnmodifiedValue;
        }
    }
}
