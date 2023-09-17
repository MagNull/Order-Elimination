using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.Infrastructure
{
    [GUIColor(0.6f, 1, 0.6f)]
    public interface IPointRelativePattern : ICloneable<IPointRelativePattern>
    {
        public Vector2Int[] GetAbsolutePositions(Vector2Int originPoint);

        public bool ContainsPositionWithOrigin(Vector2Int position, Vector2Int originPoint);
    }

    public class PointRelativePattern : IPointRelativePattern
    {
        [ShowInInspector, OdinSerialize]
        private HashSet<Vector2Int> _relativePositions = new HashSet<Vector2Int>();

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

    public class DistanceFromPointPattern : IPointRelativePattern
    {
        [HideInInspector, OdinSerialize]
        private float _minDistanceFromOrigin;
        [HideInInspector, OdinSerialize]
        private float _maxDistanceFromOrigin;

        public DistanceFromPointPattern(
            float minDistanceFromOrigin, float maxDistanceFromOrigin, bool useSquareDistance)
        {
            _minDistanceFromOrigin = minDistanceFromOrigin;
            _maxDistanceFromOrigin = maxDistanceFromOrigin;
            UseSquareDistance = useSquareDistance;
        }

        [ShowInInspector]
        public float MinDistanceFromOrigin
        {
            get => _minDistanceFromOrigin;
            set
            {
                if (value < 0) value = 0;
                if (value > MaxDistanceFromOrigin) value = MaxDistanceFromOrigin;
                _minDistanceFromOrigin = value;
            }
        }

        [ShowInInspector]
        public float MaxDistanceFromOrigin
        {
            get => _maxDistanceFromOrigin;
            set
            {
                if (value < MinDistanceFromOrigin) value = MinDistanceFromOrigin;
                _maxDistanceFromOrigin = value;
            }
        }

        [ShowInInspector, OdinSerialize]
        public bool UseSquareDistance { get; set; } = false;

        public IPointRelativePattern Clone()
        {
            var clone = new DistanceFromPointPattern(
                _minDistanceFromOrigin, _maxDistanceFromOrigin, UseSquareDistance);
            return clone;
        }

        public bool ContainsPositionWithOrigin(Vector2Int position, Vector2Int originPoint)
        {
            var pos = position - originPoint;
            if (UseSquareDistance)
            {
                var minIntDistance = Mathf.CeilToInt(MinDistanceFromOrigin);
                var maxIntDistance = Mathf.FloorToInt(MaxDistanceFromOrigin);
                return CoordinateInSymmetricalSquare(pos.x, pos.y, minIntDistance, maxIntDistance);
            }
            var sqrMagnitude = pos.sqrMagnitude;
            var minDistSqr = MinDistanceFromOrigin * MinDistanceFromOrigin;
            var maxDistSqr = MaxDistanceFromOrigin * MaxDistanceFromOrigin;
            return minDistSqr <= sqrMagnitude && sqrMagnitude <= maxDistSqr;
        }

        public Vector2Int[] GetAbsolutePositions(Vector2Int originPoint)
        {
            var filteredPoints = new List<Vector2Int>();
            var minIntDistance = Mathf.CeilToInt(MinDistanceFromOrigin);
            var maxIntDistance = Mathf.FloorToInt(MaxDistanceFromOrigin);
            for (var x = -maxIntDistance; x <= maxIntDistance; x++)
            {
                for (var y = -maxIntDistance; y <= maxIntDistance; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (UseSquareDistance)
                    {
                        if (!CoordinateInSymmetricalSquare(x, y, minIntDistance, maxIntDistance))
                            continue;
                        filteredPoints.Add(pos);
                        continue;
                    }
                    var sqrMagnitude = pos.sqrMagnitude;
                    var minDistSqr = MinDistanceFromOrigin * MinDistanceFromOrigin;
                    var maxDistSqr = MaxDistanceFromOrigin * MaxDistanceFromOrigin;
                    if (minDistSqr <= sqrMagnitude && sqrMagnitude <= maxDistSqr)
                        filteredPoints.Add(pos);
                }
            }
            return filteredPoints.Select(p => p + originPoint).ToArray();
        }

        private bool CoordinateInSymmetricalSquare(int x, int y, int minRange, int maxRange)
        {
            minRange = Mathf.Abs(minRange);
            maxRange = Mathf.Abs(maxRange);
            if (maxRange < minRange)
                throw new InvalidProgramException();
            // -max -min 0 +min +max
            var xOut = x < -maxRange || maxRange < x;
            var yOut = y < -maxRange || maxRange < y;
            var xInExcludedRange = -minRange < x && x < minRange;
            var yInExcludedRange = -minRange < y && y < minRange;
            if (xOut || yOut || xInExcludedRange && yInExcludedRange)
                return false;
            return true;
        }
    }

    public class CompoundPointPattern : IPointRelativePattern
    {
        [ShowInInspector, OdinSerialize]
        public IPointRelativePattern PatternA { get; set; }

        [ShowInInspector, OdinSerialize]
        public BooleanOperation BooleanOperation { get; set; }

        [ShowInInspector, OdinSerialize]
        public IPointRelativePattern PatternB { get; set; }

        public IPointRelativePattern Clone()
        {
            var clone = new CompoundPointPattern();
            clone.PatternA = PatternA.Clone();
            clone.PatternB = PatternB.Clone();
            clone.BooleanOperation = BooleanOperation;
            return clone;
        }

        public bool ContainsPositionWithOrigin(Vector2Int position, Vector2Int originPoint)
        {
            var offset = position - originPoint;
            var inPatternA = PatternA.ContainsPositionWithOrigin(position, offset);
            var inPatternB = PatternB.ContainsPositionWithOrigin(position, offset);
            return BooleanOperation switch
            {
                BooleanOperation.Union => inPatternA || inPatternB,
                BooleanOperation.Intersect => inPatternA && inPatternB,
                BooleanOperation.Except => inPatternA && !inPatternB,
                _ => throw new NotImplementedException(),
            };
        }

        public Vector2Int[] GetAbsolutePositions(Vector2Int originPoint)
        {
            var patternAPositions = PatternA.GetAbsolutePositions(originPoint);
            var patternBPositions = PatternB.GetAbsolutePositions(originPoint);
            var result = BooleanOperation switch
            {
                BooleanOperation.Union => patternAPositions.Union(patternBPositions),
                BooleanOperation.Intersect => patternAPositions.Intersect(patternBPositions),
                BooleanOperation.Except => patternAPositions.Except(patternBPositions),
                _ => throw new NotImplementedException(),
            };
            return result.ToArray();
        }
    }
}
