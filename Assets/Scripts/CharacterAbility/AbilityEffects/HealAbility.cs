using System;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class HealAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly DamageHealType _damageHealType;
        private readonly int _healAmount;
        private readonly AbilityScaleFrom _abilityScaleFrom;
        private readonly float _scale;

        public HealAbility(IAbilityCaster caster, Ability nextAbility, DamageHealType damageHealType, int healAmount,
            AbilityScaleFrom abilityScaleFrom, float scale) : base(caster)
        {
            _scale = scale;
            _nextAbility = nextAbility;
            _damageHealType = damageHealType;
            _healAmount = healAmount;
            _abilityScaleFrom = abilityScaleFrom;
        }

        public override void Use(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap)
        {
            BattleCharacter targetCharacter = target.GetView().GetComponent<BattleCharacterView>().Model;
            var heal = _abilityScaleFrom switch
            {
                AbilityScaleFrom.Attack => _healAmount + (int) (stats.Attack * _scale),
                AbilityScaleFrom.Health => _healAmount + (int) (stats.UnmodifiedHealth * _scale),
                AbilityScaleFrom.Movement => _healAmount + (int) (stats.UnmodifiedMovement * _scale),
                _ => throw new ArgumentOutOfRangeException()
            };
            for (var i = 0; i < _healAmount; i++)
                targetCharacter.TakeRecover(heal, stats.Accuracy, _damageHealType);

            _nextAbility?.Use(target, stats, battleMap);
        }
    }
}