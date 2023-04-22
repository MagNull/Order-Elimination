using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;

namespace OrderElimination.AbilitySystem
{
    public class SimpleCasterRelativePattern : CasterRelativePattern
    {
        [ShowInInspector, SerializeField]
        protected readonly Dictionary<int, PointRelativePattern> _relativeToCasterOffsetsByGroupId = new();

        public override CellGroupDistributionPoicy DistributionPoicy => throw new NotImplementedException();

        public override CellGroupsContainer GetAffectedCellGroups(CellRangeBorders mapBorders, Vector2Int casterPosition)
        {
            var sortedPoints = new Dictionary<int, Vector2Int[]>();
            foreach (var group in _relativeToCasterOffsetsByGroupId.Keys)
            {
                var newPoints = _relativeToCasterOffsetsByGroupId[group]
                    .GetAbsolutePositions(casterPosition)
                    .Where(p => mapBorders.Contains(p));
                sortedPoints.Add(group, newPoints.ToArray());
            }
            return new CellGroupsContainer(sortedPoints);
        }
    }
}