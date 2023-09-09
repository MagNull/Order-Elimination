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

        public bool CanBePrecalculatedWith(ValueCalculationContext context)
        {
            return context.BattleCaster != null && context.BattleCaster.BattleStats != null
                || context.MetaCaster != null && context.MetaCaster.CharacterStats != null
                || context.TemplateCharacterCaster != null
                || context.TemplateStructureCaster != null;
        }

        public IContextValueGetter Clone()
        {
            var clone = new CasterStatGetter();
            clone.CasterStat = CasterStat;
            clone.UseUnmodifiedValue = UseUnmodifiedValue;
            return clone;
        }

        public float GetValue(ValueCalculationContext context)
        {
            if (!CanBePrecalculatedWith(context))
                throw new NotEnoughDataArgumentException();
            if (context.BattleCaster != null)
            {
                return UseUnmodifiedValue
                    ? context.BattleCaster.BattleStats[CasterStat].UnmodifiedValue
                    : context.BattleCaster.BattleStats[CasterStat].ModifiedValue;
            }
            if (context.MetaCaster != null)
            {
                return context.MetaCaster.CharacterStats[CasterStat];
            }
            if (context.TemplateCharacterCaster != null)
            {
                return context.TemplateCharacterCaster.GetBaseBattleStats()[CasterStat];
            }
            if (context.TemplateStructureCaster != null)
            {
                return CasterStat == BattleStat.MaxHealth
                    ? context.TemplateStructureCaster.MaxHealth
                    : 0;
            }
            throw new InvalidProgramException();
        }
    }
}
