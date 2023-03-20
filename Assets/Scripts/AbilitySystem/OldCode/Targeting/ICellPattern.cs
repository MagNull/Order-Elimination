using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;

namespace OrderElimination.AbilitySystem
{
    //Добавить под-классы/интерфейсы для паттернов относительно кастера, относительно цели и по направлению?
    //ICasterRelativePattern
    //ITargetRelativePattern
    //IDirectionPattern
    public interface ICellPattern 
    {
        public CellTargetGroups GetAffectedCellGroups(BattleMap battleMap, Cell casterCell, Cell[] targetCells);
    }

    public class AreaPattern : ICellPattern 
    {
        public int Radius { get; set; }

        public CellTargetGroups GetAffectedCellGroups(BattleMap battleMap, Cell casterCell, Cell[] targetCells)
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
