using System;
using CharacterAbility.AbilityEffects;
using OrderElimination.BM;
using VContainer;

namespace CharacterAbility
{
    [Serializable]
    public class AbilityFactory
    {
        private readonly BattleMapView _battleMapView;
        private readonly IObjectResolver _objectResolver;

        [Inject]
        public AbilityFactory(BattleMapView battleMapView, IObjectResolver objectResolver)
        {
            _battleMapView = battleMapView;
            _objectResolver = objectResolver;
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
                    abilityInfo.ActiveParams.TargetType == TargetType.Self, BattleObjectType.None),
                AbilityInfo.AbilityType.Passive => new PassiveAbility(caster, abilityInfo.PassiveParams.MoveToTrigger,
                    abilityInfo.PassiveParams.TriggerType,
                    ability, BattleObjectType.None, 100),
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
                    BattleObjectType.None);
            }

            if (abilityInfo.ActiveParams.HasPatternTargetEffect)
            {
                ability = AddEffects(abilityInfo.ActiveParams.PatternEffects, ability, caster);
                ability = new PatternTargetAbility(caster, abilityInfo.ActiveParams.Pattern, _battleMapView.Map,
                    ability, BattleObjectType.None, abilityInfo.ActiveParams.PatternMaxDistance);
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
                        ability = new DamageAbility(caster, effectDesc.MainEffect, ability, probability,
                            _battleMapView.Map,
                            effectDesc._damageHealTarget, effectDesc.DamageType,
                            effectDesc.Amounts, effectDesc.ScaleFrom, effectDesc.Scale, effectDesc.Filter);
                        break;
                    case AbilityEffectType.Heal:
                        ability = new HealAbility(caster, effectDesc.MainEffect, ability, probability,
                            effectDesc._damageHealTarget,
                            effectDesc.Amounts,
                            effectDesc.ScaleFrom, effectDesc.Scale, effectDesc.Filter);
                        break;
                    case AbilityEffectType.Move:
                        ability = new MoveAbility(caster, effectDesc.MainEffect, ability, probability,
                            _battleMapView.Map, effectDesc.Filter,
                            effectDesc.StepDelay);
                        break;
                    case AbilityEffectType.Modificator:
                        ability = new ModificatorAbility(caster, effectDesc.MainEffect, ability, probability,
                            effectDesc.Modificator,
                            effectDesc.ModificatorValue, effectDesc.Filter);
                        break;
                    case AbilityEffectType.OverTime:
                        ability = new OverTimeAbility(caster, effectDesc.MainEffect, ability, probability,
                            effectDesc._damageHealTarget,
                            effectDesc.OverTimeType,
                            effectDesc.Duration,
                            effectDesc.TickValue, effectDesc.IsUnique, effectDesc.Filter,
                            effectDesc.EffectView);
                        break;
                    case AbilityEffectType.TickingBuff:
                        ability = new TickingBuffAbility(caster, effectDesc.MainEffect, ability, probability,
                            effectDesc.BuffType,
                            effectDesc.BuffModificator, effectDesc.ScaleFromWhom, effectDesc.Duration,
                            effectDesc.Filter,
                            effectDesc.DamageType, effectDesc.Multiplier, effectDesc.IsUnique, effectDesc.EffectView,
                            _objectResolver,
                            effectDesc.TriggerEffects);
                        break;
                    case AbilityEffectType.ConditionalBuff:
                        ability = new ConditionalBuffAbility(caster, effectDesc.MainEffect, ability, probability,
                            effectDesc.BuffType,
                            effectDesc.BuffModificator, effectDesc.Duration, effectDesc.ScaleFromWhom, effectDesc.ConditionType,
                            effectDesc.Filter,
                            effectDesc.DamageType, effectDesc.Multiplier, effectDesc.IsUnique, effectDesc.EffectView,
                            _objectResolver);
                        break;

                    case AbilityEffectType.Contreffect:
                        ability = new ContreffectAbility(caster, effectDesc.MainEffect, ability, effectDesc.Filter,
                            probability,
                            _battleMapView.Map.GetStraightDistance, effectDesc.Distance);
                        break;
                    case AbilityEffectType.ObjectSpawn:
                        ability = new ObjectSpawnAbility(caster, effectDesc.ObjectInfo,
                            _objectResolver.Resolve<EnvironmentFactory>(), effectDesc.Duration, _battleMapView.Map,
                            effectDesc.MainEffect,
                            ability, BattleObjectType.None);
                        break;
                }
            }

            return ability;
        }
    }
}