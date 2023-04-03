using System;
using Cysharp.Threading.Tasks;
using OrderElimination;

namespace CharacterAbility.AbilityEffects
{
    public class HealAbility : Ability
    {
        private readonly DamageHealTarget _damageHealTarget;
        private readonly int _healAmount;
        private readonly AbilityScaleFrom _abilityScaleFrom;
        private readonly float _scale;

        public HealAbility(IBattleObject caster, bool isMain, Ability nextEffect, float probability,
            DamageHealTarget damageHealTarget,
            int healAmount,
            AbilityScaleFrom abilityScaleFrom, float scale, BattleObjectType filter) : base(caster, isMain,
            nextEffect, filter,
            probability)
        {
            _scale = scale;
            _damageHealTarget = damageHealTarget;
            _healAmount = healAmount;
            _abilityScaleFrom = abilityScaleFrom;
        }

        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            var heal = _abilityScaleFrom switch
            {
                AbilityScaleFrom.Attack => _healAmount + (int) (stats.Attack * _scale),
                AbilityScaleFrom.Health => _healAmount + (int) (stats.UnmodifiedHealth * _scale),
                AbilityScaleFrom.Movement => _healAmount + (int) (stats.UnmodifiedMovement * _scale),
                _ => throw new ArgumentOutOfRangeException()
            };
            for (var i = 0; i < _healAmount; i++)
                target.TakeRecover(heal, stats.Accuracy, _damageHealTarget);
            await UseNext(target, stats);
        }
    }
}