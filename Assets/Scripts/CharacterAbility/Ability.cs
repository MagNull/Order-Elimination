using System;
using System.Threading.Tasks;
using CharacterAbility.AbilityEffects;
using Cysharp.Threading.Tasks;
using OrderElimination;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterAbility
{
    public abstract class Ability
    {
        private readonly Ability _nextEffect;
        protected readonly float _probability;
        protected readonly IBattleObject _caster;
        private readonly bool _isMain;
        private readonly BattleObjectType _filter;

        protected Ability(IBattleObject caster, bool isMain, Ability nextEffect, BattleObjectType filter,
            float probability = 100)
        {
            _caster = caster;
            _isMain = isMain;
            _nextEffect = nextEffect;
            _filter = filter;
            _probability = probability;
        }

        public async UniTask Use(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if (target.Type != _filter && _filter != BattleObjectType.None || Random.Range(0, 100) > _probability)
            {
                await UseNext(target, stats);
                return;
            }

            await ApplyEffect(target, stats);
        }

        protected async UniTask UseNext(IBattleObject target, IReadOnlyBattleStats stats, bool success = true)
        {
            if (_nextEffect != null && (!_isMain || success))
                await _nextEffect.Use(target, stats);
        }

        protected abstract UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats);
    }
}