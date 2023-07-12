using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class AccuracyAffectionRecord : IReadOnlyAccuracyAffectionRecord
    {
        private readonly List<Vector2Int> _affectorPositions = new();
        private readonly Dictionary<Vector2Int, List<BattleObstacle>> _affectors = new();
        private readonly Dictionary<BattleObstacle, ContextValueModificationResult> _modifications = new();

        /// <summary>
        /// Returns positions containing obstacles that modified accuracy in order of their affection.
        /// </summary>
        public IReadOnlyList<Vector2Int> AffectorPositions => _affectorPositions;

        /// <summary>
        /// Returns obstacles that modified accuracy in order of their affection.
        /// </summary>
        public IEnumerable<BattleObstacle> Affectors => _affectorPositions.SelectMany(p => _affectors[p]);

        public IContextValueGetter FinalModifiedAccuracy
        {
            get
            {
                var lastPos = _affectorPositions[_affectorPositions.Count - 1];
                return GetTotalAffectionIn(lastPos);
            }
        }

        public IContextValueGetter GetTotalAffectionIn(Vector2Int position)
        {
            var affectors = _affectors[position];
            var lastAffector = affectors[affectors.Count - 1];
            return GetAffectionBy(lastAffector).ModifiedValueGetter;
        }

        public ContextValueModificationResult GetAffectionBy(BattleObstacle obstacle)
            => _modifications[obstacle];

        public void AddAffector(BattleObstacle affector, ContextValueModificationResult influence)
        {
            var position = affector.ObstacleEntity.Position;
            if (!_affectors.ContainsKey(position))
            {
                _affectorPositions.Add(position);
                _affectors.Add(position, new());
            }
            _affectors[position].Add(affector);
            _modifications.Add(affector, influence);
        }
    }
}
