using System;
using CharacterAbility.AbilityEffects;

namespace CharacterAbility
{
    [Serializable]
    public class AbilityFactory
    {
        private readonly BattleMapView _battleMapView;

        public AbilityFactory(BattleMapView battleMapView)
        {
            _battleMapView = battleMapView;
        }

        public AbilityView CreateAbilityView(AbilityInfo abilityInfo, BattleCharacter caster)
        {
            Ability ability = CreateAbility(abilityInfo, caster);

            return new AbilityView(caster, ability, abilityInfo, _battleMapView);
        }

        public Ability CreateAbility(AbilityInfo abilityInfo, IBattleObject caster)
        {
            Ability ability = null;
            if (abilityInfo.HasAreaEffect)
            {
                ability = AddEffects(abilityInfo.AreaEffects, ability, caster);
                ability = new AreaAbility(caster, ability, _battleMapView.Map, abilityInfo.AreaRadius,
                    BattleObjectSide.None);
            }

            if (abilityInfo.HasTargetEffect)
            {
                ability = AddEffects(abilityInfo.TargetEffects, ability, caster);
            }

            ability = new TargetAbility(caster, ability, abilityInfo.TargetType == TargetType.Self,
                BattleObjectSide.None);

            return ability;
        }

        private Ability AddEffects(AbilityEffect[] effects, Ability ability, IBattleObject caster)
        {
            for (var i = effects.Length - 1; i >= 0; i--)
            {
                var effect = effects[i];
                var probability = effect.HasProbability ? effect.Probability : 100f;
                switch (effect.Type)
                {
                    case AbilityEffectType.Damage:
                        ability = new DamageAbility(caster, ability, probability, _battleMapView.Map,
                            effect._damageHealTarget, effect.Amounts,
                            effect.ScaleFrom, effect.Scale, effect.Filter);
                        break;
                    case AbilityEffectType.Heal:
                        ability = new HealAbility(caster, ability, probability, effect._damageHealTarget, effect.Amounts,
                            effect.ScaleFrom, effect.Scale, effect.Filter);
                        break;
                    case AbilityEffectType.Move:
                        ability = new MoveAbility(caster, ability, probability, _battleMapView.Map, effect.Filter);
                        break;
                    case AbilityEffectType.Modificator:
                        ability = new ModificatorAbility(caster, ability, probability, effect.Modificator,
                            effect.ModificatorValue, effect.Filter);
                        break;
                    case AbilityEffectType.OverTime:
                        ability = new OverTimeAbility(caster, ability, probability, effect._damageHealTarget,
                            effect.OverTimeType,
                            effect.Duration,
                            effect.TickValue, effect.Filter);
                        break;
                    case AbilityEffectType.Buff:
                        ability = new BuffAbility(caster, ability, probability, effect.BuffType, effect.BuffValue,
                            effect.Duration,
                            effect.Filter);
                        break;
                    case AbilityEffectType.Stun:
                        ability = new StunAbility(caster, ability, probability, effect.Filter);
                        break;
                }
            }

            return ability;
        }
    }
}