using System;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    //TODO: Think to remove this class
    public class ContreffectAbility : Ability
    {
        private readonly Ability _nextEffect;
        private readonly Func<IBattleObject, IBattleObject, int> _getDistance;
        private readonly int _distance;

        public ContreffectAbility(IBattleObject caster, Ability nextEffect, BattleObjectSide filter, float probability,
            Func<IBattleObject, IBattleObject, int> getDistance, int distance)
            : base(caster, nextEffect, filter, probability)
        {
            _nextEffect = nextEffect;
            _getDistance = getDistance;
            _distance = distance;
        }

        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if(_getDistance.Invoke(_caster, target) > _distance)
                return;
            _nextEffect.Use(target, stats);
        }
    }
}