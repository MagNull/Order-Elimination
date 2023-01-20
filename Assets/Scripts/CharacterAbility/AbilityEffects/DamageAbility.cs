using System;
using Cysharp.Threading.Tasks;
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
        private readonly AbilityScaleFrom _scaleFrom;

        public DamageAbility(IBattleObject caster, bool isMain, Ability nextEffect, float probability,
            BattleMap battleMap,
            DamageHealTarget damageHealTarget,
            DamageType damageType,
            int damageAmounts,
            AbilityScaleFrom scaleFrom,
            float attackScale, BattleObjectSide filter) :
            base(caster, isMain, nextEffect, filter, probability)
        {
            _scaleFrom = scaleFrom;
            _attackScale = attackScale;
            _damageAmounts = damageAmounts;
            _battleMap = battleMap;
            _damageHealTarget = damageHealTarget;
            _damageType = damageType;
        }

        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            var damage = ApplyScalability(target, stats, _battleMap);
            var attackInfo = new DamageInfo
            {
                Attacker = _caster,
                Accuracy = stats.Accuracy,
                Damage = damage,
                DamageType = _damageType,
                DamageHealTarget = _damageHealTarget,
                DamageModificator = stats.DamageModificator
            };
            var success = false;
            for (var i = 0; i < _damageAmounts; i++)
            {
                var info = target.TakeDamage(attackInfo);
                success = info.HealthDamage > 0 || info.ArmorDamage > 0 || success;
            }

            await UseNext(target, stats, success);
        }

        private int ApplyScalability(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap)
        {
            var damage = _scaleFrom switch
            {
                AbilityScaleFrom.Attack => (int) (stats.Attack * _attackScale),
                AbilityScaleFrom.Health => (int) (stats.UnmodifiedHealth * _attackScale),
                AbilityScaleFrom.Movement => (int) (stats.UnmodifiedMovement * _attackScale),
                AbilityScaleFrom.Distance => Mathf.RoundToInt(stats.Attack *
                    (battleMap.GetStraightDistance(_caster, target) - 1)),
                _ => throw new ArgumentOutOfRangeException()
            };
            Debug.Log(battleMap.GetStraightDistance(_caster, target) - 1);
            return damage;
        }
    }
}