using System;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class DamageAbility : Ability
    {
        private readonly Ability _nextEffect;
        private readonly BattleMap _battleMap;
        private readonly DamageHealType _damageHealType;
        private readonly int _damageAmounts;
        private readonly float _attackScale;
        private readonly AbilityScaleFrom _scaleFrom;

        public DamageAbility(IBattleObject caster, Ability nextEffect, float probability, BattleMap battleMap, DamageHealType damageHealType,
            int damageAmounts,
            AbilityScaleFrom scaleFrom,
            float attackScale, BattleObjectSide filter) :
            base(caster, nextEffect, filter, probability)
        {
            _scaleFrom = scaleFrom;
            _attackScale = attackScale;
            _damageAmounts = damageAmounts;
            _nextEffect = nextEffect;
            _battleMap = battleMap;
            _damageHealType = damageHealType;
        }

        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if (_filter == BattleObjectSide.None || target.Side == _filter)
            {
                var damage = CalculateDamage(target, stats, _battleMap);

                for (var i = 0; i < _damageAmounts; i++)
                    target.TakeDamage(damage, stats.Accuracy, _damageHealType);
            }
            _nextEffect?.Use(target, stats);
        }

        private int CalculateDamage(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap)
        {
            var damage = _scaleFrom switch
            {
                AbilityScaleFrom.Attack => (int) (stats.Attack * _attackScale),
                AbilityScaleFrom.Health => (int) (stats.UnmodifiedHealth * _attackScale),
                AbilityScaleFrom.Movement => (int) (stats.UnmodifiedMovement * _attackScale),
                AbilityScaleFrom.Distance => stats.Attack * battleMap.GetDistance(_caster, target),
                _ => throw new ArgumentOutOfRangeException()
            };
            switch (stats.AttackType)
            {
                case AttackType.DoubleArmor:
                    if (target.Stats.Armor > 0 &&
                        _damageHealType is DamageHealType.Normal or DamageHealType.OnlyArmor)
                        damage += 2 * damage > target.Stats.Armor
                            ? (int) Mathf.Floor(Mathf.Clamp(damage, 0, target.Stats.Armor)) / 2
                            : damage;
                    break;
                case AttackType.DoubleHealth:
                    if (_damageHealType == DamageHealType.Normal && target.Stats.Armor == 0 ||
                        _damageHealType == DamageHealType.OnlyHealth)
                        damage += 2 * damage > target.Stats.Health
                            ? (int) Mathf.Floor(Mathf.Clamp(damage, 0, target.Stats.Health)) / 2
                            : damage;
                    break;
                case AttackType.Normal:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return damage;
        }
    }
}