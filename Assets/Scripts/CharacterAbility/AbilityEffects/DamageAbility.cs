using System;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class DamageAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly int _damage;
        private readonly int _damageAmounts;
        private readonly float _attackScale;

        public DamageAbility(IAbilityCaster caster, Ability nextAbility, int damageAmounts, float attackScale) :
            base(caster)
        {
            _attackScale = attackScale;
            _damageAmounts = damageAmounts;
            _nextAbility = nextAbility;
        }

        public override void Use(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap)
        {
            BattleCharacter targetCharacter = target.GetView().GetComponent<BattleCharacterView>().Model;
            for (var i = 0; i < _damageAmounts; i++)
                targetCharacter.TakeDamage((int) (stats.Attack * _attackScale), stats.Accuracy);

            _nextAbility?.Use(target, stats, battleMap);
        }
    }
}