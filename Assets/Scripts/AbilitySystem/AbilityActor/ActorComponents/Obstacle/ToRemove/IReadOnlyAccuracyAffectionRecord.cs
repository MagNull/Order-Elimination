using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface IReadOnlyAccuracyAffectionRecord
    {
        /// <summary>
        /// Returns positions containing obstacles that modified accuracy in order of their affection.
        /// </summary>
        public IReadOnlyList<Vector2Int> AffectorPositions { get; }

        /// <summary>
        /// Returns obstacles that modified accuracy in order of their affection.
        /// </summary>
        public IEnumerable<BattleObstacle> Affectors { get; }

        public IContextValueGetter FinalModifiedAccuracy { get; }

        public IContextValueGetter GetTotalAffectionIn(Vector2Int position);

        public ContextValueModificationResult GetAffectionBy(BattleObstacle obstacle);
    }
}
