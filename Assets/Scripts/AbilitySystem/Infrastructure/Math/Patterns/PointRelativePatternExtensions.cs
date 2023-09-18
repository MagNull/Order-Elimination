using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.Infrastructure
{
    public static class PointRelativePatternExtensions
    {
        public static int GetMaxSquareDistance(this IPointRelativePattern pattern, Vector2Int origin)
        {
            return pattern.GetAbsolutePositions(origin)
                .Select(v => Mathf.Max(Mathf.Abs(v.x), Mathf.Abs(v.y)))
                .Max();
        }

        public static float GetMaxDistance(this IPointRelativePattern pattern, Vector2Int origin)
        {
            return pattern.GetAbsolutePositions(origin)
                .Max(p => (p - origin).magnitude);
        }
    }
}
