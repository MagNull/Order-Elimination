using System;
using Cysharp.Threading.Tasks;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class OverTimeAbility : Ability
    {
        private readonly DamageHealTarget _damageHealTarget;
        private readonly OverTimeAbilityType _overTimeAbilityType;
        private readonly int _duration;
        private readonly int _tickValue;
        private ITickEffect _tickEffect;
        private ITickEffectView _effectView;

        public OverTimeAbility(IBattleObject caster, bool isMain, Ability nextEffect, float probability,
            DamageHealTarget damageHealTarget, OverTimeAbilityType overTimeAbilityType,
            int duration, int tickValue, BattleObjectSide filter, ITickEffectView view) : base(
            caster, isMain, nextEffect, filter,
            probability)
        {
            _damageHealTarget = damageHealTarget;
            _overTimeAbilityType = overTimeAbilityType;
            _duration = duration;
            _tickValue = tickValue;
            _effectView = view;
        }

        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            var tickEffect = _overTimeAbilityType switch
            {
                OverTimeAbilityType.Damage => new DamageOverTimeEffect(_damageHealTarget, _tickValue, _duration, _effectView),
                OverTimeAbilityType.Heal => new DamageOverTimeEffect(_damageHealTarget, _tickValue, _duration, _effectView),
                _ => throw new ArgumentOutOfRangeException(nameof(_overTimeAbilityType), _overTimeAbilityType, null)
            };
            target.AddTickEffect(tickEffect);

            await UseNext(target, stats);
        }
    }
}