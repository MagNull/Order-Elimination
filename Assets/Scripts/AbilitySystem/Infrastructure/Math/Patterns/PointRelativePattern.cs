using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.Infrastructure
{
    public class PointRelativePattern : IPointRelativePattern
    {
        [ShowInInspector, OdinSerialize]
        private HashSet<Vector2Int> _relativePositions = new();

        public PointRelativePattern()
        {

        }

        public PointRelativePattern(IEnumerable<Vector2Int> relativePositions)
        {
            _relativePositions = relativePositions.ToHashSet();
        }

        public IEnumerable<Vector2Int> RelativePositions => _relativePositions;

        public bool AddRelativePosition(Vector2Int offset) => _relativePositions.Add(offset);

        public bool RemoveRelativePosition(Vector2Int offset) => _relativePositions.Remove(offset);

        public IPointRelativePattern Clone()
        {
            var clone = new PointRelativePattern();
            clone._relativePositions = _relativePositions.ToHashSet();
            return clone;
        }

        public bool ContainsPositionWithOrigin(Vector2Int position, Vector2Int originPoint)
        {
            return _relativePositions.Contains(position - originPoint);
        }

        public Vector2Int[] GetAbsolutePositions(Vector2Int originPoint)
        {
            return _relativePositions.Select(v => originPoint + v).ToArray();
        }
    }
}
