using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class CellGroupsFilter
    {
        public HashSet<int> AffectedCellGroups;

        public List<Vector2Int> GetFilteredCells(CellGroupsContainer groupsContainer)
        {
            var filteredGroupIds = groupsContainer.CellGroups.Keys.Where(gId => AffectedCellGroups.Contains(gId));
            return filteredGroupIds.Select(id => groupsContainer.CellGroups[id]).SelectMany(p => p).ToList();
        }
    }
}
