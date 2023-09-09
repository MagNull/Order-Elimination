using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
    public struct DynamicStatGetter : IContextValueGetter
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
            if (!CanBePrecalculatedWith(context))
                throw new NotEnoughDataArgumentException();
            var entity = Entity switch
            {
                ActionEntity.Caster => context.BattleCaster,
                ActionEntity.Target => context.BattleTarget,
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

        public bool CanBePrecalculatedWith(ValueCalculationContext context)
        {
            return Entity switch
            {
                ActionEntity.Caster => context.BattleCaster != null && context.BattleCaster.BattleStats != null,
                ActionEntity.Target => context.BattleTarget != null && context.BattleTarget.BattleStats != null,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
