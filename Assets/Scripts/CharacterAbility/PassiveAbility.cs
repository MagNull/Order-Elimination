using System;
using CharacterAbility.AbilityEffects;
using OrderElimination;
using OrderElimination.Battle;
using OrderElimination.BattleMap;
using UnityEngine;

namespace CharacterAbility
{
    public class PassiveAbility : Ability
    {
        private readonly PassiveAbilityParams.PassiveTriggerType _passiveTriggerType;
        private readonly Ability _nextEffect;

        public PassiveAbility(IBattleObject caster, PassiveAbilityParams.PassiveTriggerType passiveTriggerType,
            Ability nextEffect, BattleObjectSide filter, float probability) : base(caster, nextEffect, filter,
            probability)
        {
            _passiveTriggerType = passiveTriggerType;
            _nextEffect = nextEffect;
        }

        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            switch (_passiveTriggerType)
            {
                case PassiveAbilityParams.PassiveTriggerType.Damage:
                    target.Damaged += info =>
                    {
                        if (info.CancelType != DamageCancelType.None)
                            return;
                        if(_nextEffect is ContreffectAbility)
                            _nextEffect.Use(info.Attacker, _caster.Stats);
                        else
                            _nextEffect?.Use(target, stats);
                    };
                    break;
                case PassiveAbilityParams.PassiveTriggerType.Movement:
                    target.Moved += (from, to) =>
                    {
                        if(to.GetObject() is EnvironmentObject)
                            _nextEffect?.Use(target, stats);
                    };
                    break;
                case PassiveAbilityParams.PassiveTriggerType.Spawn:
                    _nextEffect?.Use(target, stats);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}