using System.Collections.Generic;
using OrderElimination;

namespace CharacterAbility.BuffEffects
{
    public interface IBuffTarget
    {
        public IReadOnlyList<ITickEffect> AllEffects { get; }
        public IReadOnlyBattleStats Stats { get; }
    }
}