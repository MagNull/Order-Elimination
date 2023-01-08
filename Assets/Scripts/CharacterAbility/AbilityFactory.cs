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
            Debug.Log(abilityInfo == null);
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
                var effect = effects[i];
                var probability = effect.HasProbability ? effect.Probability : 100f;
                switch (effect.Type)
                {
                    case AbilityEffectType.Damage:
                        ability = new DamageAbility(caster, ability, probability, _battleMapView.Map,
                            effect._damageHealTarget, effect.DamageType,
                            effect.Amounts, effect.ScaleFrom, effect.Scale, effect.Filter);
                        break;
                    case AbilityEffectType.Heal:
                        ability = new HealAbility(caster, ability, probability, effect._damageHealTarget,
                            effect.Amounts,
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
                            effect.TickValue, effect.Filter,
                            effect.OverTimeType == OverTimeAbilityType.Damage ? effect.DamageType : DamageType.None);
                        break;
                    case AbilityEffectType.TickingBuff:
                        ability = new TickingBuffAbility(caster, ability, probability, effect.BuffType,
                            effect.BuffValue, effect.Duration, effect.Filter, effect.DamageType);
                        break;
                    case AbilityEffectType.ConditionalBuff:
                        ability = new ConditionalBuffAbility(caster, ability, probability, effect.BuffType,
                            effect.BuffValue, effect.ConditionType, effect.Filter, effect.DamageType);
                        break;
                    case AbilityEffectType.Stun:
                        ability = new StunAbility(caster, ability, probability, effect.Filter);
                        break;

                    case AbilityEffectType.Contreffect:
                        ability = new ContreffectAbility(caster, ability, effect.Filter, probability,
                            _battleMapView.Map.GetDistance, effect.Distance);
                        break;
                }
            }

            return ability;
        }
    }
}