using System;
using CharacterAbility.BuffEffects;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class TickingBuffAbility : Ability
    {
        private readonly BuffType _buffType;
        private readonly int _value;
        private readonly int _duration;
        private ITickEffect _buff;

        public TickingBuffAbility(IBattleObject caster, Ability effects, float probability, BuffType buffType,
            int value,
            int duration, BattleObjectSide filter) :
            base(caster, effects, filter, probability)
        {
            _buffType = buffType;
            _value = value;
            _duration = duration;
        }

        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if (_filter != BattleObjectSide.None && target.Side != _filter) 
                return;
            
            InitBuff();
            target.AddTickEffect(_buff);
        }

        private void InitBuff()
        {
            _buff = _buffType switch
            {
                BuffType.Evasion => new EvasionStatsBuff(_value, _duration),
                BuffType.Attack => new EvasionStatsBuff(_value, _duration),
                BuffType.Health => new EvasionStatsBuff(_value, _duration),
                BuffType.Movement => new EvasionStatsBuff(_value, _duration),
                BuffType.IncomingAccuracy => new IncomingBuff(IncomingDebuffType.Accuracy, _duration,
                    _value),
                BuffType.IncomingAttack => new IncomingBuff(IncomingDebuffType.Attack, _duration,
                    _value),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}