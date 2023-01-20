using System;
using CharacterAbility.AbilityEffects;
using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.Battle;
using OrderElimination.BM;
using UnityEngine;

namespace CharacterAbility
{
    public class PassiveAbility : Ability
    {
        private readonly PassiveAbilityParams.PassiveTriggerType _passiveTriggerType;
        private readonly Ability _nextEffect;

        public PassiveAbility(IBattleObject caster, PassiveAbilityParams.PassiveTriggerType passiveTriggerType,
            Ability nextEffect, BattleObjectSide filter, float probability) : base(caster, false, nextEffect, filter,
            probability)
        {
            _passiveTriggerType = passiveTriggerType;
            _nextEffect = nextEffect;
        }

        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
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