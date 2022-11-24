using System;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class OverTimeAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly DamageHealTarget _damageHealTarget;
        private readonly OverTimeAbilityType _overTimeAbilityType;
        private readonly int _duration;
        private readonly int _tickValue;
        private ITickEffect _tickEffect;

        public OverTimeAbility(IBattleObject caster, Ability nextAbility, float probability,
            DamageHealTarget damageHealTarget,
            OverTimeAbilityType overTimeAbilityType, int duration, int tickValue, BattleObjectSide filter) : base(caster, nextAbility, filter,
            probability)
        {
            _nextAbility = nextAbility;
            _damageHealTarget = damageHealTarget;
            _overTimeAbilityType = overTimeAbilityType;
            _duration = duration;
            _tickValue = tickValue;
        }

        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if (_filter == BattleObjectSide.None || target.Side == _filter)
            {
                var tickEffect = _overTimeAbilityType switch
                {
                    OverTimeAbilityType.Damage => new DamageOverTimeEffect(_damageHealTarget, _tickValue, _duration),
                    OverTimeAbilityType.Heal => new DamageOverTimeEffect(_damageHealTarget, _tickValue, _duration),
                    _ => throw new ArgumentOutOfRangeException(nameof(_overTimeAbilityType), _overTimeAbilityType, null)
                };
                target.AddTickEffect(tickEffect);
            }

            _nextAbility?.Use(target, stats);
        }
    }
}