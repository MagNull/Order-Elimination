using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class TargetRelativeCellSelector : ICellSelector
    {
        [ShowInInspector, SerializeField]
        public IPointRelativePattern RelativeToTargetOffsets { get; private set; }

        public Vector2Int[] GetCellPositions(CellSelectorContext context)
        {
            var cellPositions = new List<Vector2Int>();
            foreach (var pos in context.SelectedCellPositions)
            {
                cellPositions.AddRange(RelativeToTargetOffsets.GetAbsolutePositions(pos));
            }
            return cellPositions.ToArray();
        }
    }
}
