using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface ICellSelector
    {
        public Vector2Int[] GetCellPositions(CellSelectorContext context);
    }
}
