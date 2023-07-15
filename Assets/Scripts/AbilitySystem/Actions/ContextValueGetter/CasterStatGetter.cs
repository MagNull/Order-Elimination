using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
    [Serializable]
    public struct CasterStatGetter : IContextValueGetter
    {
        public CasterStatGetter(BattleStat stat, bool useUnmodified = false)
        {
            CasterStat = stat;
            UseUnmodifiedValue = useUnmodified;
        }

        [OdinSerialize]
        public BattleStat CasterStat { get; private set; }

        [OdinSerialize]
        public bool UseUnmodifiedValue { get; private set; }

        public string DisplayedFormula => $"Caster.{CasterStat}({(UseUnmodifiedValue ? "orig" : "mod")})";

        public IContextValueGetter Clone()
        {
            var clone = new CasterStatGetter();
            clone.CasterStat = CasterStat;
            clone.UseUnmodifiedValue = UseUnmodifiedValue;
            return clone;
        }

        public float GetValue(ActionContext useContext)
        {
            if (useContext.ActionMaker == null
                || !useContext.ActionMaker.BattleStats.HasParameter(CasterStat))
                return 0;
            if (!UseUnmodifiedValue)
                return useContext.ActionMaker.BattleStats[CasterStat].ModifiedValue;
            else
                return useContext.ActionMaker.BattleStats[CasterStat].UnmodifiedValue;
        }
    }
}
