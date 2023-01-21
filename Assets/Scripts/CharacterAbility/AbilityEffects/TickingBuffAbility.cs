using System;
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
        private readonly ITickEffectView _tickEffectView;
        private readonly IObjectResolver _objectResolver;
        private ITickEffect _buff;

        //TODO: Refactor DI in BuffAbilities
        public TickingBuffAbility(IBattleObject caster, bool isMain, Ability nextEffect, float probability,
            Buff_Type buffType,
            float value, ScaleFromWhom scaleFromWhom, int duration, BattleObjectSide filter, DamageType damageType,
            bool isMultiplier, ITickEffectView tickEffectView, IObjectResolver objectResolver) :
            base(caster, isMain, nextEffect, filter, probability)
        {
            _buffType = buffType;
            _value = value;
            _scaleFromWhom = scaleFromWhom;
            _duration = duration;
            _damageType = damageType;
            _isMultiplier = isMultiplier;
            _tickEffectView = tickEffectView;
            _objectResolver = objectResolver;
        }

        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            InitBuff();
            target.AddTickEffect(_buff);
            await UseNext(target, stats);
        }

        private void InitBuff()
        {
            _buff = _buffType switch
            {
                Buff_Type.Evasion => new StatsBuffEffect(Buff_Type.Evasion, _value, _scaleFromWhom, _duration,
                    _isMultiplier, _caster, _tickEffectView),
                Buff_Type.Accuracy => new StatsBuffEffect(Buff_Type.Accuracy, _value, _scaleFromWhom, _duration,
                    _isMultiplier, _caster, _tickEffectView),
                Buff_Type.Attack => new StatsBuffEffect(Buff_Type.Attack, _value, _scaleFromWhom, _duration,
                    _isMultiplier,
                    _caster, _tickEffectView),
                Buff_Type.Health => new StatsBuffEffect(Buff_Type.Health, _value, _scaleFromWhom, _duration,
                    _isMultiplier,
                    _caster, _tickEffectView),
                Buff_Type.Movement => new StatsBuffEffect(Buff_Type.Movement, _value, _scaleFromWhom, _duration,
                    _isMultiplier, _caster, _tickEffectView),
                Buff_Type.AdditionalArmor => new StatsBuffEffect(Buff_Type.AdditionalArmor, _value, _scaleFromWhom,
                    _duration,
                    _isMultiplier, _caster, _tickEffectView),
                Buff_Type.IncomingAccuracy => new IncomingBuff(Buff_Type.IncomingAccuracy, _duration,
                    _value, _tickEffectView, _damageType),
                Buff_Type.IncomingDamageIncrease => new IncomingBuff(Buff_Type.IncomingDamageIncrease, _duration,
                    _value, _tickEffectView, _damageType),
                Buff_Type.IncomingDamageReduction => new IncomingBuff(Buff_Type.IncomingDamageReduction, _duration,
                    _value, _tickEffectView, _damageType),
                Buff_Type.Concealment => new ConcealmentBuff(_duration, _objectResolver.Resolve<CharactersBank>(),
                    _tickEffectView),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}