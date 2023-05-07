using OrderElimination.AbilitySystem;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using OrderElimination.Infrastructure;
using System.Collections.Generic;
using System;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public enum CellGroupDistributionPoicy
    {
        /// <summary>
        /// Допускает относить одну клетку к нескольким группам, разрешая относить её к одной и той же несколько раз.
        /// Н: cell1=[0,0,1], cell2=[0], cell3=[1,2,2], ...
        /// </summary>
        AllowMultipleGroupsWithDuplicates,
        /// <summary>
        /// Допускает относить одну клетку к нескольким группам, но в единстевнном экземпляре. 
        /// Н: cell1=[0,1], cell2=[0], cell3=[1,2], ...
        /// </summary>
        AllowMultipleGroupsInSingleInstance,
        /// <summary>
        /// Допускает относить клетку только к одной единственной группе с наивысшим приоритетом. 
        /// Н: cell1=[0], cell2=[0], cell3=[1], ...
        /// </summary>
        OnlySingleGroupWithHighestPriority
    }

    public abstract class CellGroupDistributionPattern 
    {
        public abstract CellGroupDistributionPoicy DistributionPoicy { get; }

        public abstract CellGroupsContainer GetAffectedCellGroups(
            CellRangeBorders mapBorders, 
            Vector2Int casterPosition, 
            params Vector2Int[] targetPositions);

        protected void AddPositionsConsideringDistributionPolicy(
            Dictionary<int, Vector2Int[]> positions,
            int targetGroup,
            IEnumerable<Vector2Int> newPositions)
        {
            if (!positions.ContainsKey(targetGroup))
                positions.Add(targetGroup, newPositions.ToArray());
            else
            {
                switch (DistributionPoicy)
                {
                    case CellGroupDistributionPoicy.AllowMultipleGroupsWithDuplicates:
                        positions[targetGroup] = positions[targetGroup].Concat(newPositions).ToArray();
                        break;
                    case CellGroupDistributionPoicy.AllowMultipleGroupsInSingleInstance:
                        positions[targetGroup] = positions[targetGroup].Union(newPositions).ToArray();
                        break;
                    case CellGroupDistributionPoicy.OnlySingleGroupWithHighestPriority:
                        throw new NotImplementedException();
                        break;
                }
            }
        }
    }

    public abstract class CasterRelativePattern : CellGroupDistributionPattern
    {
        public abstract CellGroupsContainer GetAffectedCellGroups(CellRangeBorders mapBorders, Vector2Int casterPosition);

        public override CellGroupsContainer GetAffectedCellGroups(
            CellRangeBorders mapBorders, 
            Vector2Int casterPosition, 
            Vector2Int[] targetPositions)
            => GetAffectedCellGroups(mapBorders, casterPosition);
    }

    public abstract class TargetRelativePattern : CellGroupDistributionPattern
    {
        public abstract CellGroupsContainer GetAffectedCellGroups(CellRangeBorders mapBorders, params Vector2Int[] targetPositions);

        public override CellGroupsContainer GetAffectedCellGroups(
            CellRangeBorders mapBorders, 
            Vector2Int casterPosition, 
            Vector2Int[] targetPositions)
            => GetAffectedCellGroups(mapBorders, targetPositions);
    }

    public abstract class CasterToTargetRelativePattern : CellGroupDistributionPattern
    {

    }
}