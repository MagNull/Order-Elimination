using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class ExecutionGroupsFilter
    {
        private bool affectMainCells;
        private bool affectAreaCells;
        private Dictionary<int, bool> affectedSecondaryCells; //int secondaryCellGroupID, bool isAffected

        public List<Vector2Int> GetFilteredCells(AbilityExecutionGroups cellGroups)
        {
            var affectedCells = new List<Vector2Int>();
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
