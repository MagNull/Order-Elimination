﻿using System;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class OverTimeAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly OverTimeAbilityType _overTimeAbilityType;
        private readonly int _duration;
        private readonly int _tickValue;
        private ITickEffect _tickEffect;

        public OverTimeAbility(IAbilityCaster caster, Ability nextAbility, OverTimeAbilityType overTimeAbilityType,
            int duration, int tickValue) : base(caster)
        {
            _nextAbility = nextAbility;
            _overTimeAbilityType = overTimeAbilityType;
            Debug.Log(_duration);
            _duration = duration;
            _tickValue = tickValue;
        }

        public override void Use(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap)
        {
            var tickEffect = _overTimeAbilityType switch
            {
                OverTimeAbilityType.Damage => new DamageOverTimeEffect(target, _tickValue, _duration),
                OverTimeAbilityType.Heal => new DamageOverTimeEffect(target, _tickValue, _duration),
                _ => throw new ArgumentOutOfRangeException(nameof(_overTimeAbilityType), _overTimeAbilityType, null)
            };
            target.AddTickEffect(tickEffect);
            
            _nextAbility?.Use(target, stats, battleMap);
        }
    }
}