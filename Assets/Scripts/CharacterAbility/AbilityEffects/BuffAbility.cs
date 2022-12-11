using System;
using CharacterAbility.BuffEffects;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class BuffAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly BuffType _buffType;
        private readonly int _value;
        private readonly int _duration;

        public BuffAbility(IBattleObject caster, Ability nextAbility, float probability, BuffType buffType, int value,
            int duration, BattleObjectSide filter) :
            base(caster, nextAbility, filter, probability)
        {
            _nextAbility = nextAbility;
            _buffType = buffType;
            _value = value;
            _duration = duration;
        }

        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if (_filter == BattleObjectSide.None || target.Side == _filter)
            {
                ITickEffect buff = _buffType switch
                {
                    BuffType.Evasion => new EvasionStatsBuff(_value, _duration),
                    BuffType.Attack => new EvasionStatsBuff(_value, _duration),
                    BuffType.Health => new EvasionStatsBuff(_value, _duration),
                    BuffType.Movement => new EvasionStatsBuff( _value, _duration),
                    BuffType.IncomingAccuracy => new IncomingBuff(IncomingDebuffType.Accuracy, _duration,
                        _value),
                    BuffType.IncomingAttack => new IncomingBuff(IncomingDebuffType.Attack, _duration,
                        _value),
                    _ => throw new ArgumentOutOfRangeException()
                };
                target.AddTickEffect(buff);
            }

            _nextAbility?.Use(target, stats);
        }
    }
}