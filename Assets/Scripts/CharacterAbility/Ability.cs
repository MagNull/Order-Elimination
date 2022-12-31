using System;
using CharacterAbility.AbilityEffects;
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

        public float Probability => _probability;

        protected Ability(IBattleObject caster, Ability nextEffect, BattleObjectSide filter,
            float probability = 100)
        {
            _caster = caster;
            _nextEffect = nextEffect;
            _filter = filter;
            _probability = probability;
        }

        public void Use(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if (target.Side != _filter && _filter != BattleObjectSide.None &&
                Random.value > _probability / 100)
            {
                _nextEffect?.Use(target, stats);
                return;
            }
            ApplyEffect(target, stats);
        }

        protected abstract void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats);
    }
}