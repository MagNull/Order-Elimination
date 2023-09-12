using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    [Tooltip("Returns all cells within Battle Map")]
    public class WholeBattleMapCellSelector : ICellSelector
    {
        public Vector2Int[] GetCellPositions(CellSelectorContext context)
        {
            return context.BattleContext.BattleMap.CellRangeBorders.EnumerateCellPositions().ToArray();
        }
    }
}
