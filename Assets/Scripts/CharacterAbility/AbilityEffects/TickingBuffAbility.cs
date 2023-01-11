using System;
using CharacterAbility.BuffEffects;
using Cysharp.Threading.Tasks;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class TickingBuffAbility : Ability
    {
        private readonly Ability _nextEffect;
        private readonly Buff_Type _buffType;
        private readonly int _value;
        private readonly int _duration;
        private readonly DamageType _damageType;
        private ITickEffect _buff;

        public TickingBuffAbility(IBattleObject caster, Ability nextEffect, float probability, Buff_Type buffType,
            int value, int duration, BattleObjectSide filter, DamageType damageType) :
            base(caster, nextEffect, filter, probability)
        {
            _nextEffect = nextEffect;
            _buffType = buffType;
            _value = value;
            _duration = duration;
            _damageType = damageType;
        }

        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            InitBuff();
            target.AddTickEffect(_buff);
            if(_nextEffect == null)
                return;
            await _nextEffect.Use(target, stats);
        }

        private void InitBuff()
        {
            _buff = _buffType switch
            {
                Buff_Type.Evasion => new EvasionStatsBuff(_value, _duration),
                Buff_Type.Attack => new AttackStatsBuff(_value, _duration),
                Buff_Type.Health => new EvasionStatsBuff(_value, _duration),
                Buff_Type.Accuracy => new AccuracyStatsBuff(_value, _duration),
                Buff_Type.Movement => new EvasionStatsBuff(_value, _duration),
                Buff_Type.IncomingAccuracy => new IncomingBuff(Buff_Type.IncomingAccuracy, _duration,
                    _value, _damageType),
                Buff_Type.IncomingDamageIncrease => new IncomingBuff(Buff_Type.IncomingDamageIncrease, _duration,
                    _value, _damageType),
                Buff_Type.IncomingDamageReduction => new IncomingBuff(Buff_Type.IncomingDamageReduction, _duration,
                    _value, _damageType),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}