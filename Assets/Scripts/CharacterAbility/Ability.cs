using System;
using CharacterAbility.AbilityEffects;
using OrderElimination;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterAbility
{
    public abstract class Ability
    {
        private readonly Ability _effects;
        private readonly float _probability;
        protected readonly IBattleObject _caster;
        protected readonly BattleObjectSide _filter;

        protected Ability(IBattleObject caster, Ability effects, BattleObjectSide filter,
            float probability)
        {
            _caster = caster;
            _effects = effects;
            _filter = filter;
            _probability = probability;
        }

        public void Use(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if (Random.value * 100 <= _probability)
            {
                ApplyEffect(target, stats);
            }

            if (this is not ActiveAbility and not PassiveAbility)
            {
                _effects?.Use(target, stats);
            }
        }

        protected abstract void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats);
    }
}