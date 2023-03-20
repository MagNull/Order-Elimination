using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class CellTargetGroupFilter
    {
        private bool affectMainCells;
        private bool affectAreaCells;
        private Dictionary<int, bool> affectedSecondaryCells; //int secondaryCellGroupID, bool isAffected

        public List<Cell> GetFilteredCells(CellTargetGroups cellGroups)
        {
            var affectedCells = new List<Cell>();
            if (affectMainCells)
                affectedCells = affectedCells.Concat(cellGroups.MainTargets).ToList();
            if (affectAreaCells)
                affectedCells = affectedCells.Concat(cellGroups.AreaTargets).ToList();
            for (var i = 0; i < cellGroups.SecondaryTargetGroups.Length; i++)
            {
                affectedCells = affectedCells.Concat(cellGroups.SecondaryTargetGroups[i]).ToList();
            }
            return affectedCells;
        }
    }
}
