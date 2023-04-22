using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public readonly struct CellGroupsContainer
    {
        public readonly IReadOnlyDictionary<int, Vector2Int[]> CellGroups;

        public CellGroupsContainer(IReadOnlyDictionary<int, Vector2Int[]> cellGroups)
        {
            if (cellGroups == null)
                throw new ArgumentException();
            CellGroups = cellGroups;
        }

        //TODO учитывать CellGroupDistributionPolicy
        public static CellGroupsContainer Add(CellGroupsContainer container1, CellGroupsContainer container2)
        {
            var combinedCellGroups = container1.CellGroups.ToDictionary(e => e.Key, e => e.Value.ToArray());
            foreach (var gId in container2.CellGroups.Keys)
            {
                if (combinedCellGroups.ContainsKey(gId))
                    combinedCellGroups[gId] = combinedCellGroups[gId].Concat(container2.CellGroups[gId].ToArray()).ToArray();
                else
                    combinedCellGroups.Add(gId, container2.CellGroups[gId].ToArray());
            }
            return new CellGroupsContainer(combinedCellGroups);
        }

        //public static CellGroupsContainer operator +(CellGroupsContainer container1, CellGroupsContainer container2)
        //    => Add(container1, container2);
    }
}
