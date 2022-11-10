using System;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class DamageAbility : Ability
    {
        private readonly Ability _nextEffect;
        private readonly DamageHealType _damageHealType;
        private readonly int _damageAmounts;
        private readonly float _attackScale;
        private readonly AbilityScaleFrom _scaleFrom;

        public DamageAbility(IAbilityCaster caster, Ability nextEffect, DamageHealType damageHealType,
            int damageAmounts,
            AbilityScaleFrom scaleFrom,
            float attackScale, BattleObjectSide filter) :
            base(caster, filter)
        {
            _scaleFrom = scaleFrom;
            _attackScale = attackScale;
            _damageAmounts = damageAmounts;
            _nextEffect = nextEffect;
            _damageHealType = damageHealType;
        }

        public override void Use(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap)
        {
            if (_filter == BattleObjectSide.None || target.Side == _filter)
            {
                var damage = CalculateDamage(target, stats, battleMap);

                for (var i = 0; i < _damageAmounts; i++)
                    target.TakeDamage(damage, stats.Accuracy, _damageHealType);
            }
            _nextEffect?.Use(target, stats, battleMap);
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