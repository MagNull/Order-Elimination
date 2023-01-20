using System;
using CharacterAbility.AbilityEffects;
using UnityEngine;

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

        public AbilityView CreateAbilityView(AbilityInfo abilityInfo, BattleCharacter caster,
            BattleCharacterView casterView)
        {
            var ability = CreateAbility(abilityInfo, caster);
            if (abilityInfo.Type == AbilityInfo.AbilityType.Passive)
            {
                ability.Use(caster, caster.Stats);
            }

            return new AbilityView(caster, ability, abilityInfo, _battleMapView, casterView);
        }

        public Ability CreateAbility(AbilityInfo abilityInfo, IBattleObject caster)
        {
            Ability ability = null;
            switch (abilityInfo.Type)
            {
                case AbilityInfo.AbilityType.Active:
                    ability = ApplyActiveEffects(abilityInfo, caster, ability);
                    break;
                case AbilityInfo.AbilityType.Passive:
                    ability = AddEffects(abilityInfo.PassiveParams.Effects, ability, caster);
                    break;
            }

            ability = abilityInfo.Type switch
            {
                AbilityInfo.AbilityType.Active => new ActiveAbility(caster, ability,
                    abilityInfo.ActiveParams.TargetType == TargetType.Self, BattleObjectSide.None),
                AbilityInfo.AbilityType.Passive => new PassiveAbility(caster, abilityInfo.PassiveParams.TriggerType,
                    ability, BattleObjectSide.None, 100),
                _ => throw new Exception("Unknown ability type")
            };

            return ability;
        }

        private Ability ApplyActiveEffects(AbilityInfo abilityInfo, IBattleObject caster, Ability ability)
        {
            if (abilityInfo.ActiveParams.HasAreaEffect)
            {
                ability = AddEffects(abilityInfo.ActiveParams.AreaEffects, ability, caster);
                ability = new AreaAbility(caster, ability, _battleMapView.Map, abilityInfo.ActiveParams.AreaRadius,
                    BattleObjectSide.None);
            }

            if (abilityInfo.ActiveParams.HasTargetEffect)
            {
                ability = AddEffects(abilityInfo.ActiveParams.TargetEffects, ability, caster);
            }

            return ability;
        }

        private Ability AddEffects(AbilityEffect[] effects, Ability ability, IBattleObject caster)
        {
            for (var i = effects.Length - 1; i >= 0; i--)
            {
                var effectDesc = effects[i];
                var probability = effectDesc.HasProbability ? effectDesc.Probability : 100f;
                switch (effectDesc.Type)
                {
                    case AbilityEffectType.Damage:
                        ability = new DamageAbility(caster, effectDesc.MainEffect, ability, probability, _battleMapView.Map,
                            effectDesc._damageHealTarget, effectDesc.DamageType,
                            effectDesc.Amounts, effectDesc.ScaleFrom, effectDesc.Scale, effectDesc.Filter);
                        break;
                    case AbilityEffectType.Heal:
                        ability = new HealAbility(caster, effectDesc.MainEffect, ability, probability, effectDesc._damageHealTarget,
                            effectDesc.Amounts,
                            effectDesc.ScaleFrom, effectDesc.Scale, effectDesc.Filter);
                        break;
                    case AbilityEffectType.Move:
                        ability = new MoveAbility(caster, effectDesc.MainEffect, ability, probability, _battleMapView.Map, effectDesc.Filter,
                            effectDesc.StepDelay);
                        break;
                    case AbilityEffectType.Modificator:
                        ability = new ModificatorAbility(caster, effectDesc.MainEffect, ability, probability, effectDesc.Modificator,
                            effectDesc.ModificatorValue, effectDesc.Filter);
                        break;
                    case AbilityEffectType.OverTime:
                        ability = new OverTimeAbility(caster, effectDesc.MainEffect, ability, probability, effectDesc._damageHealTarget,
                            effectDesc.OverTimeType,
                            effectDesc.Duration,
                            effectDesc.TickValue, effectDesc.Filter,
                            effectDesc.OverTimeType == OverTimeAbilityType.Damage ? effectDesc.DamageType : DamageType.None,
                            effectDesc.EffectView);
                        break;
                    case AbilityEffectType.TickingBuff:
                        ability = new TickingBuffAbility(caster, effectDesc.MainEffect, ability, probability, effectDesc.BuffType,
                            effectDesc.BuffModificator, effectDesc.ScaleFromWhom, effectDesc.Duration, effectDesc.Filter,
                            effectDesc.DamageType, effectDesc.Multiplier, effectDesc.EffectView);
                        break;
                    case AbilityEffectType.ConditionalBuff:
                        ability = new ConditionalBuffAbility(caster, effectDesc.MainEffect, ability, probability, effectDesc.BuffType,
                            effectDesc.BuffModificator, effectDesc.ScaleFromWhom, effectDesc.ConditionType, effectDesc.Filter,
                            effectDesc.DamageType, effectDesc.Multiplier, effectDesc.EffectView);
                        break;
                    case AbilityEffectType.Stun:
                        ability = new StunAbility(caster, effectDesc.MainEffect, ability, probability, effectDesc.Filter);
                        break;

                    case AbilityEffectType.Contreffect:
                        ability = new ContreffectAbility(caster, effectDesc.MainEffect, ability, effectDesc.Filter, probability,
                            _battleMapView.Map.GetStraightDistance, effectDesc.Distance);
                        break;
                }
            }

            return ability;
        }
    }
}