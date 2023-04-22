﻿using System;
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

        [ShowInInspector, SerializeField]
        protected readonly Dictionary<int, PointRelativePattern> _relativeToCasterOffsetsByGroupId = new();

        [ShowInInspector, SerializeField]
        protected readonly Dictionary<int, PointRelativePattern> _relativeToTargetOffsetsByGroupId = new();

        public override CellGroupsContainer GetAffectedCellGroups(CellRangeBorders mapBorders, Vector2Int casterPosition, params Vector2Int[] targetPositions)
        {
            var sortedPoints = new Dictionary<int, Vector2Int[]>();
            foreach (var targetPos in targetPositions)
            {
                foreach (var group in _relativeToCasterOffsetsByGroupId.Keys)
                {
                    var newPoints = _relativeToCasterOffsetsByGroupId[group]
                        .GetAbsolutePositions(casterPosition)
                        .Where(p => mapBorders.Contains(p));
                    sortedPoints.Add(group, newPoints.ToArray());
                }
                foreach (var group in _relativeToTargetOffsetsByGroupId.Keys)
                {
                    var newPoints = _relativeToTargetOffsetsByGroupId[group].GetAbsolutePositions(targetPos).Where(p => mapBorders.Contains(p));
                    if (!sortedPoints.ContainsKey(group))
                        sortedPoints.Add(group, newPoints.ToArray());
                    else
                    {
                        switch (DistributionPoicy)
                        {
                            case CellGroupDistributionPoicy.AllowMultipleGroupsWithDuplicates:
                                sortedPoints[group] = sortedPoints[group].Concat(newPoints).ToArray();
                                break;
                            case CellGroupDistributionPoicy.AllowMultipleGroupsInSingleInstance:
                                sortedPoints[group] = sortedPoints[group].Union(newPoints).ToArray();
                                break;
                            case CellGroupDistributionPoicy.OnlySingleGroupWithHighestPriority:
                                throw new NotImplementedException();
                                break;
                        }
                    }
                }
            }
            return new CellGroupsContainer(sortedPoints);
        }
    }
}