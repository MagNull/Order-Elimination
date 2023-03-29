using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using static OrderElimination.Infrastructure.CellMath;

namespace OrderElimination.AbilitySystem
{
    //Добавить под-классы/интерфейсы для паттернов относительно кастера, относительно цели и по направлению?
    //ICasterRelativePattern
    //ITargetRelativePattern
    //IDirectionPattern
    public interface ICellPattern 
    {
        public AbilityExecutionGroups GetAffectedCellGroups(CellRangeBorders mapBorders, Vector2Int casterPosition, Vector2Int[] positionsInput);
    }

    public interface ICasterRelativePattern : ICellPattern
    {
        public AbilityExecutionGroups GetAffectedCellGroups(CellRangeBorders mapBorders, Vector2Int casterPosition);
    }

    public class SingleTargetCellPattern : ICellPattern
    {
        public AbilityExecutionGroups GetAffectedCellGroups(CellRangeBorders mapBorders, Vector2Int casterPosition, Vector2Int[] positionsInput)
        {
            if (positionsInput.Length == 0) throw new ArgumentException($"{nameof(positionsInput)} must contain at least 1 element.");
            var mainTargets = new Vector2Int[1];
            mainTargets[0] = positionsInput[0];
            return new AbilityExecutionGroups(mainTargets);
        }
    }

    public class AreaPattern : ICellPattern 
    {
        public int Radius { get; set; }

        public AbilityExecutionGroups GetAffectedCellGroups(CellRangeBorders mapBorders, Vector2Int casterPosition, Vector2Int[] positionsInput)
        {
            var mainTargets = new List<Cell>();
            var areaTargets = new List<Cell>();
            //if (!battleMap.Cells.Contains(casterCell) || !battleMap.Cells.Contains(targetCell))
            //    return new ArgumentException();
            //Vector2Int casterPos = battleMap.GetCellCoordinate(casterCell);
            //foreach (var target in targetCells)
            //{
            //    Vector2Int targetPos = battleMap.GetCellCoordinate(casterCell);
            //    ...
            //}
            return new CellTargetGroups(mainTargets.ToArray(), areaTargets.ToArray());
        }
    }
}
