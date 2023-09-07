using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
    public class DynamicStatGetter : IContextValueGetter
    {
        public enum DynamicBattleStat
        {
            Health,
            PureArmor,
            TemporaryArmor,
            TotalArmor
        }

        [ShowInInspector, OdinSerialize]
        public ActionEntity Entity { get; set; }

        [ShowInInspector, OdinSerialize]
        public DynamicBattleStat DynamicStat { get; set; }

        public string DisplayedFormula => $"{Entity}.{DynamicStat}";

        public IContextValueGetter Clone()
        {
            var clone = new DynamicStatGetter();
            clone.Entity = Entity;
            clone.DynamicStat = DynamicStat;
            return clone;
        }

        public float GetValue(ValueCalculationContext context)
        {
            var entity = Entity switch
            {
                ActionEntity.Caster => context.Caster,
                ActionEntity.Target => context.Target,
                _ => throw new NotImplementedException(),
            };
            return DynamicStat switch
            {
                DynamicBattleStat.Health => entity.BattleStats.Health,
                DynamicBattleStat.PureArmor => entity.BattleStats.PureArmor,
                DynamicBattleStat.TemporaryArmor => entity.BattleStats.TemporaryArmor,
                DynamicBattleStat.TotalArmor => entity.BattleStats.TotalArmor,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
