using System;
using CharacterAbility.BuffEffects;
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
        private ITickEffectView _effectView;

        public TickingBuffAbility(IBattleObject caster, Ability nextEffect, float probability, Buff_Type buffType,
            int value, int duration, BattleObjectSide filter, DamageType damageType, ITickEffectView effectView) :
            base(caster, nextEffect, filter, probability)
        {
            _nextEffect = nextEffect;
            _buffType = buffType;
            _value = value;
            _duration = duration;
            _damageType = damageType;
            _effectView = effectView;
        }

        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            InitBuff();
            target.AddTickEffect(_buff);
            _nextEffect?.Use(target, stats);
        }

        private void InitBuff()
        {
            ITickEffectView effectView = _effectView;
            _buff = _buffType switch
            {
                Buff_Type.Evasion => new EvasionStatsBuff(_value, _duration, effectView),
                Buff_Type.Attack => new AttackStatsBuff(_value, _duration, effectView),
                Buff_Type.Health => new EvasionStatsBuff(_value, _duration, effectView),
                Buff_Type.Accuracy => new AccuracyStatsBuff(_value, _duration, effectView),
                Buff_Type.Movement => new EvasionStatsBuff(_value, _duration, effectView),
                Buff_Type.IncomingAccuracy => new IncomingBuff(Buff_Type.IncomingAccuracy, _duration,
                    _value, effectView, _damageType),
                Buff_Type.IncomingDamageIncrease => new IncomingBuff(Buff_Type.IncomingDamageIncrease, _duration,
                    _value, effectView, _damageType),
                Buff_Type.IncomingDamageReduction => new IncomingBuff(Buff_Type.IncomingDamageReduction, _duration,
                    _value, effectView, _damageType),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}