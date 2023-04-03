using System;
using Cysharp.Threading.Tasks;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    //TODO: Think to remove this class
    public class ContreffectAbility : Ability
    {
        private readonly Func<IBattleObject, IBattleObject, int> _getDistance;
        private readonly int _distance;

        public ContreffectAbility(IBattleObject caster, bool isMain, Ability nextEffect, BattleObjectType filter, float probability,
            Func<IBattleObject, IBattleObject, int> getDistance, int distance)
            : base(caster,isMain , nextEffect, filter, probability)
        {
            _getDistance = getDistance;
            _distance = distance;
        }

        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if(_getDistance.Invoke(_caster, target) > _distance)
                return;
            await UseNext(target, stats);
        }
    }
}