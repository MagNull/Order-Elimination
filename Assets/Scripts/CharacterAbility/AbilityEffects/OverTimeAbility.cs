using System;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class OverTimeAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly DamageHealType _damageHealType;
        private readonly OverTimeAbilityType _overTimeAbilityType;
        private readonly int _duration;
        private readonly int _tickValue;
        private ITickEffect _tickEffect;

        public OverTimeAbility(IAbilityCaster caster, Ability nextAbility, DamageHealType damageHealType,
            OverTimeAbilityType overTimeAbilityType,
            int duration, int tickValue, BattleObjectSide filter) : base(caster, filter)
        {
            _nextAbility = nextAbility;
            _damageHealType = damageHealType;
            _overTimeAbilityType = overTimeAbilityType;
            _duration = duration;
            _tickValue = tickValue;
        }

        public override void Use(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap)
        {
            if (_filter == BattleObjectSide.None || target.Side == _filter)
            {
                var tickEffect = _overTimeAbilityType switch
                {
                    OverTimeAbilityType.Damage => new DamageTickEffect(target, _damageHealType, _tickValue, _duration),
                    OverTimeAbilityType.Heal => new DamageTickEffect(target, _damageHealType, _tickValue, _duration),
                    _ => throw new ArgumentOutOfRangeException(nameof(_overTimeAbilityType), _overTimeAbilityType, null)
                };
                target.AddTickEffect(tickEffect);
            }
            
            _nextAbility?.Use(target, stats, battleMap);
        }
    }
}