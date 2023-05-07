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
        [ShowInInspector, SerializeField, DictionaryDrawerSettings(KeyLabel = "Group", ValueLabel = "Pattern")]
        protected Dictionary<int, IPointRelativePattern> _relativeToCasterOffsets = new();

        public override CellGroupDistributionPoicy DistributionPoicy => throw new NotImplementedException();

        public override CellGroupsContainer GetAffectedCellGroups(CellRangeBorders mapBorders, Vector2Int casterPosition)
        {
            var sortedPoints = new Dictionary<int, Vector2Int[]>();
            foreach (var group in _relativeToCasterOffsets.Keys)
            {
                var newPoints = _relativeToCasterOffsets[group]
                    .GetAbsolutePositions(casterPosition)
                    .Where(p => mapBorders.Contains(p));
                AddPositionsConsideringDistributionPolicy(sortedPoints, group, newPoints);
            }
            return new CellGroupsContainer(sortedPoints);
        }
    }
}