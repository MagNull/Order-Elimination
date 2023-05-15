using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;

namespace OrderElimination.AbilitySystem
{
    public class SimpleTargetRelativePattern : TargetRelativePattern
    {
        public override CellGroupDistributionPoicy DistributionPoicy => CellGroupDistributionPoicy.AllowMultipleGroupsWithDuplicates;

        //Запоминаем позиции относительно кастера
        [ShowInInspector, SerializeField, DictionaryDrawerSettings(KeyLabel = "Group", ValueLabel = "Pattern")]
        protected Dictionary<int, IPointRelativePattern> _relativeToTargetOffsets = new();

        public override CellGroupsContainer GetAffectedCellGroups(CellRangeBorders mapBorders, Vector2Int[] targetPositions)
        {
            var sortedPoints = new Dictionary<int, Vector2Int[]>();
            foreach (var targetPos in targetPositions)
            {
                foreach (var group in _relativeToTargetOffsets.Keys)
                {
                    var newPoints = _relativeToTargetOffsets[group].GetAbsolutePositions(targetPos).Where(p => mapBorders.Contains(p));
                    AddPositionsConsideringDistributionPolicy(sortedPoints, group, newPoints);
                }
            }
            return new CellGroupsContainer(sortedPoints);
        }
    }
}