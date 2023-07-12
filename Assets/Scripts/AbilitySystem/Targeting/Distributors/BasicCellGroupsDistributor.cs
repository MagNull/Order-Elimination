using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class BasicCellGroupsDistributor : ICellGroupsDistributor
    {
        [DictionaryDrawerSettings(KeyLabel = "Group", ValueLabel = "Cell Selector")]
        [ShowInInspector, OdinSerialize]
        private Dictionary<int, ICellSelector> _groupSelectors = new();

        public CellGroupsContainer DistributeSelection(
            IBattleContext battleContext, 
            AbilitySystemActor askingEntity, 
            IEnumerable<Vector2Int> selectedPositions)
        {
            var mapPositions = battleContext.BattleMap.CellRangeBorders.EnumerateCellPositions();
            var cellSelectorContext = new CellSelectorContext(
                battleContext, mapPositions, askingEntity, selectedPositions.ToArray());
            var distributedCells = new Dictionary<int, Vector2Int[]>();
            foreach (var group in _groupSelectors.Keys)
            {
                distributedCells.Add(
                    group, _groupSelectors[group].GetCellPositions(cellSelectorContext));
            }
            return new CellGroupsContainer(distributedCells);
        }
    }
}
