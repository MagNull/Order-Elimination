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

        public BuffAbility(IAbilityCaster caster, Ability nextAbility, BuffType buffType, int value, int duration,
            BattleObjectSide filter) :
            base(caster, filter)
        {
            _nextAbility = nextAbility;
            _buffType = buffType;
            _value = value;
            _duration = duration;
        }

        public override void Use(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap)
        {
            if (_filter == BattleObjectSide.None || target.Side == _filter)
            {
                ITickEffect buff = _buffType switch
                {
                    BuffType.Evasion => new EvasionStatsBuff(target, _value, _duration),
                    BuffType.Attack => new EvasionStatsBuff(target, _value, _duration),
                    BuffType.Health => new EvasionStatsBuff(target, _value, _duration),
                    BuffType.Movement => new EvasionStatsBuff(target, _value, _duration),
                    BuffType.IncomingAccuracy => new IncomingDebuff(target, IncomingDebuffType.Accuracy, _duration,
                        _value),
                    BuffType.IncomingAttack => new IncomingDebuff(target, IncomingDebuffType.Attack, _duration,
                        _value),
                    _ => throw new ArgumentOutOfRangeException()
                };
                target.AddTickEffect(buff);
            }

            _nextAbility?.Use(target, stats, battleMap);
        }
    }
}