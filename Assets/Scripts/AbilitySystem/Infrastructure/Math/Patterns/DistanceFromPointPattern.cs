﻿using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.Infrastructure
{
    public class DistanceFromPointPattern : IPointRelativePattern
    {
        [HideInInspector, OdinSerialize]
        private float _minDistanceFromOrigin;
        [HideInInspector, OdinSerialize]
        private float _maxDistanceFromOrigin;

        public DistanceFromPointPattern()
        {
            _minDistanceFromOrigin = 0;
            _maxDistanceFromOrigin = 0;
            UseSquareDistance = true;
        }

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
}