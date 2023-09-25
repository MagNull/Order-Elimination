using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;
using UnityEngine;

namespace OrderElimination.Infrastructure
{
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
