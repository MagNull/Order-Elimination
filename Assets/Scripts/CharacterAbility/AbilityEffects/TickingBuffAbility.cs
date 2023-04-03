using System;
using System.Linq;
using CharacterAbility.BuffEffects;
using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.Battle;
using UnityEngine;
using VContainer;

namespace CharacterAbility.AbilityEffects
{
    public class TickingBuffAbility : Ability
    {
        private readonly Buff_Type _buffType;
        private readonly float _value;
        private readonly ScaleFromWhom _scaleFromWhom;
        private readonly int _duration;
        private readonly DamageType _damageType;
        private readonly bool _isMultiplier;
        private readonly bool _isUnique;
        private readonly ITickEffectView _tickEffectView;
        private readonly IObjectResolver _objectResolver;
        private readonly ITickEffect[] _triggerEffects;
        private ITickEffect _buff;

        //TODO: Refactor DI in BuffAbilities
        public TickingBuffAbility(IBattleObject caster, bool isMain, Ability nextEffect, float probability,
            Buff_Type buffType,
            float value, ScaleFromWhom scaleFromWhom, int duration, BattleObjectType filter, DamageType damageType,
            bool isMultiplier, bool isUnique, ITickEffectView tickEffectView, IObjectResolver objectResolver,
            ITickEffect[] triggerEffects) :
            base(caster, isMain, nextEffect, filter, probability)
        {
            _buffType = buffType;
            _value = value;
            _scaleFromWhom = scaleFromWhom;
            _duration = duration;
            _damageType = damageType;
            _isMultiplier = isMultiplier;
            _isUnique = isUnique;
            _tickEffectView = tickEffectView;
            _objectResolver = objectResolver;
            _triggerEffects = triggerEffects; //TODO: Remove in Refactoring
        }

        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            InitBuff();
            if (_buff.IsUnique && target.AllEffects.Any(ef => ef.Equals(_buff)))
            {
                await UseNext(target, stats);
                return;
            }
            
            target.AddTickEffect(_buff);
            await UseNext(target, stats);
        }

        private void InitBuff()
        {
            _buff = _buffType switch
            {
                Buff_Type.Evasion => new StatsBuffEffect(_isUnique, Buff_Type.Evasion, _value, _scaleFromWhom,
                    _duration,
                    _isMultiplier, _caster, _tickEffectView),
                Buff_Type.Accuracy => new StatsBuffEffect(_isUnique, Buff_Type.Accuracy, _value, _scaleFromWhom,
                    _duration,
                    _isMultiplier, _caster, _tickEffectView),
                Buff_Type.Attack => new StatsBuffEffect(_isUnique, Buff_Type.Attack, _value, _scaleFromWhom, _duration,
                    _isMultiplier,
                    _caster, _tickEffectView),
                Buff_Type.Health => new StatsBuffEffect(_isUnique, Buff_Type.Health, _value, _scaleFromWhom, _duration,
                    _isMultiplier,
                    _caster, _tickEffectView),
                Buff_Type.Movement => new StatsBuffEffect(_isUnique, Buff_Type.Movement, _value, _scaleFromWhom,
                    _duration,
                    _isMultiplier, _caster, _tickEffectView),
                Buff_Type.AdditionalArmor => new StatsBuffEffect(_isUnique, Buff_Type.AdditionalArmor, _value,
                    _scaleFromWhom,
                    _duration,
                    _isMultiplier, _caster, _tickEffectView),
                Buff_Type.IncomingAccuracy => new IncomingBuff(_isUnique, Buff_Type.IncomingAccuracy, _duration,
                    _value, _tickEffectView, _damageType),
                Buff_Type.IncomingDamageIncrease => new IncomingBuff(_isUnique, Buff_Type.IncomingDamageIncrease,
                    _duration,
                    _value, _tickEffectView, _damageType),
                Buff_Type.IncomingDamageReduction => new IncomingBuff(_isUnique, Buff_Type.IncomingDamageReduction,
                    _duration,
                    _value, _tickEffectView, _damageType),
                Buff_Type.Concealment => new ConcealmentBuff(_duration, _isUnique,
                    _objectResolver.Resolve<CharactersBank>(),
                    _tickEffectView),
                Buff_Type.OutcomingAttack => new OutcomingBuff(_isUnique, Buff_Type.OutcomingAttack, _value, 9999,
                    _tickEffectView,
                    _triggerEffects),
                Buff_Type.OutcomingAccuracy => new OutcomingBuff(_isUnique, Buff_Type.OutcomingAccuracy, _value, 9999,
                    _tickEffectView,
                    _triggerEffects),
                Buff_Type.Stun => new StunBuff(_duration, _tickEffectView, _isUnique),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}