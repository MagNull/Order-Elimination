using System;
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
        private readonly float _probability;
        protected readonly IBattleObject _caster;
        protected readonly BattleObjectSide _filter;

        protected Ability(IBattleObject caster, Ability nextEffect, BattleObjectSide filter,
            float probability = 100)
        {
            _caster = caster;
            _nextEffect = nextEffect;
            _filter = filter;
            _probability = probability;
        }

        public async UniTask Use(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if (target.Side != _filter && _filter != BattleObjectSide.None || Random.Range(0, 100) > _probability)
            {
                if (_nextEffect != null)
                    await _nextEffect.Use(target, stats);
                return;
            }

            await ApplyEffect(target, stats);
        }

        protected abstract UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats);
    }
}