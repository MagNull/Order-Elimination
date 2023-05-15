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
using Sirenix.OdinInspector;
using Sirenix.Serialization;

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

    [Tooltip("Defines how target groups calculated based on caster's and targets' positions.")]
    public abstract class CellGroupDistributionPattern 
    {
        public abstract CellGroupDistributionPoicy DistributionPoicy { get; }

        //[DictionaryDrawerSettings(KeyLabel = "Group", ValueLabel = "Condition")]
        //[ShowInInspector, OdinSerialize]
        //protected Dictionary<int, ICellCondition[]> CellConditions { get; set; } = new();

        public CellGroupsContainer GetAffectedCellGroups(
            CellRangeBorders mapBorders,
            Vector2Int casterPosition,
            params Vector2Int[] targetPositions)
        {
            var filteredGroups = CalculateAffectedCellGroups(mapBorders, casterPosition, targetPositions);
            //foreach(var group in CellConditions.Keys)
            //{
            //    //Needs BattleContext and caster to check conditions
            //    filteredGroups = filteredGroups.Filter(
            //        p => CellConditions[group].All(c => c.IsConditionMet(null, null, casterPosition)));
            //}
            return filteredGroups;
        }

        protected abstract CellGroupsContainer CalculateAffectedCellGroups(
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

        protected override CellGroupsContainer CalculateAffectedCellGroups(
            CellRangeBorders mapBorders, 
            Vector2Int casterPosition, 
            Vector2Int[] targetPositions)
            => GetAffectedCellGroups(mapBorders, casterPosition);
    }

    public abstract class TargetRelativePattern : CellGroupDistributionPattern
    {
        public abstract CellGroupsContainer GetAffectedCellGroups(CellRangeBorders mapBorders, params Vector2Int[] targetPositions);

        protected override CellGroupsContainer CalculateAffectedCellGroups(
            CellRangeBorders mapBorders, 
            Vector2Int casterPosition, 
            Vector2Int[] targetPositions)
            => GetAffectedCellGroups(mapBorders, targetPositions);
    }

    public abstract class CasterToTargetRelativePattern : CellGroupDistributionPattern
    {

    }
}