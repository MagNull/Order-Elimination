using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.Infrastructure
{
    public enum PatternBooleanOperation
    {
        Union,
        Intersect,
        Except
    }

    public interface IPointRelativePattern
    {
        public Vector2Int[] GetAbsolutePositions(Vector2Int originPoint);
    }

    public class PointRelativePattern : IPointRelativePattern
    {
        public IEnumerable<Vector2Int> RelativePositions => _relativePositions;
        [ShowInInspector, OdinSerialize]
        private HashSet<Vector2Int> _relativePositions = new HashSet<Vector2Int>();

        public Vector2Int[] GetAbsolutePositions(Vector2Int originPoint)
        {
            return _relativePositions.Select(v => originPoint + v).ToArray();
        }

        public bool AddRelativePosition(Vector2Int offset) => _relativePositions.Add(offset);

        public bool RemoveRelativePosition(Vector2Int offset) => _relativePositions.Remove(offset);
    }

    public class DistanceFromPointPattern : IPointRelativePattern
    {
        [HideInInspector, OdinSerialize]
        private float _minDistanceFromOrigin;
        [HideInInspector, OdinSerialize]
        private float _maxDistanceFromOrigin;

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

        public Vector2Int[] GetAbsolutePositions(Vector2Int originPoint)
        {
            var filteredPoints = new List<Vector2Int>();
            var maxIntDistance = Mathf.FloorToInt(MaxDistanceFromOrigin);
            var minIntDistance = Mathf.CeilToInt(MinDistanceFromOrigin);
            for (var x = -maxIntDistance; x <= maxIntDistance; x++)
            {
                for (var y = -maxIntDistance; y <= maxIntDistance; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (UseSquareDistance)
                    {
                        if (!CoordinateInSymmetricalRanges(x, y, minIntDistance, maxIntDistance))
                            continue;
                        filteredPoints.Add(pos);
                        continue;
                    }
                    var sqrMagnitude = pos.sqrMagnitude;
                    var minDistSqr = MinDistanceFromOrigin * MinDistanceFromOrigin;
                    var maxDistSqr = MaxDistanceFromOrigin * MaxDistanceFromOrigin;
                    if (sqrMagnitude >= minDistSqr && sqrMagnitude <= maxDistSqr)
                        filteredPoints.Add(pos);
                }
            }
            return filteredPoints.Select(p => p + originPoint).ToArray();

            bool CoordinateInSymmetricalRanges(int x, int y, int minRange, int maxRange)
            {
                minRange = Mathf.Abs(minRange);
                maxRange = Mathf.Abs(maxRange);
                // -max -min 0 +min +max
                var xOut = x > maxRange || x < -maxRange;
                var yOut = y > maxRange || y < -maxRange;
                var xIn = x < minRange && x > -minRange;
                var yIn = y < minRange && y > -minRange;
                if (xOut && yOut || xIn && yIn)
                    return false;
                return true;
            }
        }
    }

    public class CompoundPointPattern : IPointRelativePattern
    {
        [ShowInInspector, OdinSerialize]
        public IPointRelativePattern PatternA { get; set; }

        [ShowInInspector, OdinSerialize]
        public PatternBooleanOperation BooleanOperation { get; set; }

        [ShowInInspector, OdinSerialize]
        public IPointRelativePattern PatternB { get; set; }

        public Vector2Int[] GetAbsolutePositions(Vector2Int originPoint)
        {
            var patternAPositions = PatternA.GetAbsolutePositions(originPoint);
            var patternBPositions = PatternB.GetAbsolutePositions(originPoint);
            var result = BooleanOperation switch
            {
                PatternBooleanOperation.Union => patternAPositions.Union(patternBPositions),
                PatternBooleanOperation.Intersect => patternAPositions.Intersect(patternBPositions),
                PatternBooleanOperation.Except => patternAPositions.Except(patternBPositions),
                _ => throw new NotImplementedException(),
            };
            return result.ToArray();
        }
    }
}
