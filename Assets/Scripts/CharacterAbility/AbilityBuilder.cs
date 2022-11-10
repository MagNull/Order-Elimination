using System;
using CharacterAbility.AbilityEffects;
using UnityEngine;

namespace CharacterAbility
{
    [Serializable]
    public class AbilityBuilder
    {
        [SerializeField]
        private BattleMapView _battleMapView;

        public AbilityView Create(AbilityInfo abilityInfo, BattleCharacter caster)
        {
            Ability ability = null;
            if (abilityInfo.HasAreaEffect)
            {
                ability = AddEffects(abilityInfo.AreaEffects, ability, caster);
                ability = new AreaAbility(caster, ability, abilityInfo.AreaRadius, BattleObjectSide.None);
            }

            if (abilityInfo.HasTargetEffect)
            {
                ability = AddEffects(abilityInfo.TargetEffects, ability, caster);
            }

            ability = new TargetAbility(caster, ability, abilityInfo.TargetType == TargetType.Self,
                BattleObjectSide.None);

            return new AbilityView(caster, ability, abilityInfo, _battleMapView);
        }

        //TODO: Refactor damage heal type semantic
        private static Ability AddEffects(AbilityEffect[] effects, Ability ability, BattleCharacter caster)
        {
            for (var i = effects.Length - 1; i >= 0; i--)
            {
                var effect = effects[i];
                switch (effect.Type)
                {
                    case AbilityEffectType.Damage:
                        ability = new DamageAbility(caster, ability, effect.DamageHealType, effect.Amounts,
                            effect.ScaleFrom, effect.Scale, effect.Filter);
                        break;
                    case AbilityEffectType.Heal:
                        ability = new HealAbility(caster, ability, effect.DamageHealType, effect.Amounts,
                            effect.ScaleFrom, effect.Scale, effect.Filter);
                        break;
                    case AbilityEffectType.Move:
                        ability = new MoveAbility(caster, ability, effect.Filter);
                        break;
                    case AbilityEffectType.Modificator:
                        ability = new ModificatorAbility(caster, ability, effect.Modificator,
                            effect.ModificatorValue, effect.Filter);
                        break;
                    case AbilityEffectType.OverTime:
                        ability = new OverTimeAbility(caster, ability, effect.DamageHealType, effect.OverTimeType,
                            effect.Duration,
                            effect.TickValue, effect.Filter);
                        break;
                    case AbilityEffectType.Buff:
                        ability = new BuffAbility(caster, ability, effect.BuffType, effect.BuffValue, effect.Duration,
                            effect.Filter);
                        break;
                }
            }

            return ability;
        }
    }
}