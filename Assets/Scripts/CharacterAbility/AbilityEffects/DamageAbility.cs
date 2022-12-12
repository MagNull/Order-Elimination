using System;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class DamageAbility : Ability
    {
        private readonly BattleMap _battleMap;
        private readonly DamageHealTarget _damageHealTarget;
        private readonly DamageType _damageType;
        private readonly int _damageAmounts;
        private readonly float _attackScale;
        private readonly Ability _nextEffect;
        private readonly AbilityScaleFrom _scaleFrom;

        public DamageAbility(IBattleObject caster, Ability nextEffect, float probability, BattleMap battleMap,
            DamageHealTarget damageHealTarget,
            DamageType damageType,
            int damageAmounts,
            AbilityScaleFrom scaleFrom,
            float attackScale, BattleObjectSide filter) :
            base(caster, nextEffect, filter, probability)
        {
            _nextEffect = nextEffect;
            _scaleFrom = scaleFrom;
            _attackScale = attackScale;
            _damageAmounts = damageAmounts;
            _battleMap = battleMap;
            _damageHealTarget = damageHealTarget;
            _damageType = damageType;
        }

        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            var damage = ApplyScalability(target, stats, _battleMap);
            var attackInfo = new DamageInfo
            {
                Attacker = _caster,
                Accuracy = stats.Accuracy,
                Damage = damage,
                DamageType = _damageType,
                DamageHealTarget = _damageHealTarget
            };

            for (var i = 0; i < _damageAmounts; i++)
                target.TakeDamage(attackInfo);
            _nextEffect?.Use(target, stats);
        }

        private int ApplyScalability(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap)
        {
            var damage = _scaleFrom switch
            {
                AbilityScaleFrom.Attack => (int) (stats.Attack * _attackScale),
                AbilityScaleFrom.Health => (int) (stats.UnmodifiedHealth * _attackScale),
                AbilityScaleFrom.Movement => (int) (stats.UnmodifiedMovement * _attackScale),
                AbilityScaleFrom.Distance => stats.Attack * battleMap.GetDistance(_caster, target),
                _ => throw new ArgumentOutOfRangeException()
            };

            return damage;
        }
    }
}