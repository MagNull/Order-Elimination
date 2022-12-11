using System;
using OrderElimination;
using Random = UnityEngine.Random;

namespace CharacterAbility
{
    public abstract class Ability
    {
        private readonly Ability _nextAbility;
        private readonly float _probability;
        protected readonly IBattleObject _caster;
        protected readonly BattleObjectSide _filter;

        protected Ability(IBattleObject caster, Ability nextAbility, BattleObjectSide filter,
            float probability)
        {
            _caster = caster;
            _nextAbility = nextAbility;
            _filter = filter;
            _probability = probability;
        }

        public void Use(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if (Random.value * 100 <= _probability)
            {
                ApplyEffect(target, stats);
            }
            else
            {
                _nextAbility?.Use(target, stats);
            }
        }

        protected abstract void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats);
    }
}