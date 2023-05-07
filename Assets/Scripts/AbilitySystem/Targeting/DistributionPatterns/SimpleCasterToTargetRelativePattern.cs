using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;

namespace OrderElimination.AbilitySystem
{
    public class SimpleCasterToTargetRelativePattern : CasterToTargetRelativePattern
    {
        public override CellGroupDistributionPoicy DistributionPoicy => CellGroupDistributionPoicy.AllowMultipleGroupsWithDuplicates;

        [ShowInInspector, SerializeField, DictionaryDrawerSettings(KeyLabel = "Group", ValueLabel = "Pattern")]
        protected Dictionary<int, IPointRelativePattern> _relativeToCasterOffsets = new();

        [ShowInInspector, SerializeField, DictionaryDrawerSettings(KeyLabel = "Group", ValueLabel = "Pattern")]
        protected Dictionary<int, IPointRelativePattern> _relativeToTargetOffsets = new();

        [ShowInInspector, SerializeField, DictionaryDrawerSettings(KeyLabel = "Group", ValueLabel = "Pattern")]
        protected Dictionary<int, IVectorRelativePattern> _vectorRelativeOffsets = new();

        public override CellGroupsContainer GetAffectedCellGroups(CellRangeBorders mapBorders, Vector2Int casterPosition, params Vector2Int[] targetPositions)
        {
            var sortedPoints = new Dictionary<int, Vector2Int[]>();
            foreach (var targetPos in targetPositions)
            {
                foreach (var group in _relativeToCasterOffsets.Keys)
                {
                    var newPoints = _relativeToCasterOffsets[group]
                        .GetAbsolutePositions(casterPosition)
                        .Where(p => mapBorders.Contains(p));
                    AddPositionsConsideringDistributionPolicy(sortedPoints, group, newPoints);
                }
                foreach (var group in _relativeToTargetOffsets.Keys)
                {
                    var newPoints = _relativeToTargetOffsets[group]
                        .GetAbsolutePositions(targetPos)
                        .Where(p => mapBorders.Contains(p));
                    AddPositionsConsideringDistributionPolicy(sortedPoints, group, newPoints);
                }
                foreach (var group in _vectorRelativeOffsets.Keys)
                {
                    var newPoints = _vectorRelativeOffsets[group]
                        .GetAbsolutePositions(casterPosition, targetPos)
                        .Where(p => mapBorders.Contains(p));
                    AddPositionsConsideringDistributionPolicy(sortedPoints, group, newPoints);
                }
            }
            return new CellGroupsContainer(sortedPoints);
        }
    }
}