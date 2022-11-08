using System;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class DamageAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly int _damageAmounts;
        private readonly float _attackScale;
        private AbilityScaleFrom _scaleFrom;

        public DamageAbility(IAbilityCaster caster, Ability nextAbility, int damageAmounts, AbilityScaleFrom scaleFrom,
            float attackScale) :
            base(caster)
        {
            _scaleFrom = scaleFrom;
            _attackScale = attackScale;
            _damageAmounts = damageAmounts;
            _nextAbility = nextAbility;
        }

        public override void Use(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap)
        {
            BattleCharacter targetCharacter = target.GetView().GetComponent<BattleCharacterView>().Model;
            var damage = _scaleFrom switch
            {
                AbilityScaleFrom.Attack => (int) (stats.Attack * _attackScale),
                AbilityScaleFrom.Health => (int) (stats.UnmodifiedHealth * _attackScale),
                AbilityScaleFrom.Movement => (int) (stats.UnmodifiedMovement * _attackScale),
                _ => throw new ArgumentOutOfRangeException()
            };

            for (var i = 0; i < _damageAmounts; i++)
                targetCharacter.TakeDamage(damage, stats.Accuracy);

            _nextAbility?.Use(target, stats, battleMap);
        }
    }
}