using System;
using OrderElimination;

namespace CharacterAbility.AbilityEffects
{
    public class HealAbility : Ability
    {
        private readonly DamageHealTarget _damageHealTarget;
        private readonly int _healAmount;
        private readonly AbilityScaleFrom _abilityScaleFrom;
        private readonly Ability _nextEffect;
        private readonly float _scale;

        public HealAbility(IBattleObject caster, Ability nextEffect, float probability, DamageHealTarget damageHealTarget,
            int healAmount,
            AbilityScaleFrom abilityScaleFrom, float scale, BattleObjectSide filter) : base(caster, nextEffect, filter, probability)
        {
            _nextEffect = nextEffect;
            _scale = scale;
            _damageHealTarget = damageHealTarget;
            _healAmount = healAmount;
            _abilityScaleFrom = abilityScaleFrom;
        }

        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            BattleCharacter targetCharacter = target.View.GetComponent<BattleCharacterView>().Model;
            var heal = _abilityScaleFrom switch
            {
                AbilityScaleFrom.Attack => _healAmount + (int) (stats.Attack * _scale),
                AbilityScaleFrom.Health => _healAmount + (int) (stats.UnmodifiedHealth * _scale),
                AbilityScaleFrom.Movement => _healAmount + (int) (stats.UnmodifiedMovement * _scale),
                _ => throw new ArgumentOutOfRangeException()
            };
            for (var i = 0; i < _healAmount; i++)
                targetCharacter.TakeRecover(heal, stats.Accuracy, _damageHealTarget);
            _nextEffect?.Use(target, stats);
        }
    }
}