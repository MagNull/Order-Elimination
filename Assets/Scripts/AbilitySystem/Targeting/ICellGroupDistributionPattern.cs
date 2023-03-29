using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using OrderElimination.Infrastructure;

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

    public interface ICellGroupDistributionPattern 
    {
        public CellGroupDistributionPoicy DistributionPoicy { get; }

        public CellGroupsContainer GetAffectedCellGroups(
            CellRangeBorders mapBorders, 
            Vector2Int casterPosition, 
            Vector2Int[] targetPositions);
    }

    //Сделать классы не абстрактными?

    public abstract class CasterRelativePattern : ICellGroupDistributionPattern
    {
        public abstract CellGroupDistributionPoicy DistributionPoicy { get; }
        //Запоминаем позиции относительно кастера
        protected Dictionary<int, PointRelativePattern> _relativeOffsetsByGroupId = new Dictionary<int, PointRelativePattern>();
        public abstract CellGroupsContainer GetAffectedCellGroups(CellRangeBorders mapBorders, Vector2Int casterPosition);

        public CellGroupsContainer GetAffectedCellGroups(
            CellRangeBorders mapBorders, 
            Vector2Int casterPosition, 
            Vector2Int[] targetPositions)
            => GetAffectedCellGroups(mapBorders, casterPosition);
    }

    public abstract class TargetRelativePattern : ICellGroupDistributionPattern
    {
        public abstract CellGroupDistributionPoicy DistributionPoicy { get; }
        //Запоминаем позиции относительно цели

        public abstract CellGroupsContainer GetAffectedCellGroups(CellRangeBorders mapBorders, Vector2Int[] targetPositions);

        public CellGroupsContainer GetAffectedCellGroups(
            CellRangeBorders mapBorders, 
            Vector2Int casterPosition, 
            Vector2Int[] targetPositions)
            => GetAffectedCellGroups(mapBorders, targetPositions);
    }

    public abstract class CasterToTargetRelativePattern : ICellGroupDistributionPattern
    {
        public abstract CellGroupDistributionPoicy DistributionPoicy { get; }

        public abstract CellGroupsContainer GetAffectedCellGroups(
            CellRangeBorders mapBorders, 
            Vector2Int casterPosition, 
            Vector2Int[] targetPositions);
    }
}
